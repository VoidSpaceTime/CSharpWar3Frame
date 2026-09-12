using System.Diagnostics;
using System.Text;

namespace War3FrameBuild.Execution;

internal sealed record ProcessRequest(string FileName, IReadOnlyList<string> Arguments,
    string? WorkingDirectory = null, TimeSpan? Timeout = null);

internal sealed record ProcessResult(int? ExitCode, string Output, string Error, bool Canceled = false)
{
    public bool Success => !Canceled && ExitCode == 0;
}

internal interface IProcessRunner
{
    Task<ProcessResult> RunAsync(ProcessRequest request, CancellationToken cancellationToken = default);
}

/// <summary>拥有单次子进程生命周期，同时排空输出；诊断只保留固定大小尾部。</summary>
internal sealed class ProcessRunner : IProcessRunner
{
    internal const int MaxOutputChars = 32 * 1024;

    public async Task<ProcessResult> RunAsync(ProcessRequest request, CancellationToken cancellationToken = default)
    {
        using var lifetime = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        lifetime.CancelAfter(request.Timeout ?? TimeSpan.FromMinutes(3));
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo(request.FileName)
            {
                UseShellExecute = false, CreateNoWindow = true,
                RedirectStandardOutput = true, RedirectStandardError = true,
                WorkingDirectory = request.WorkingDirectory ?? Environment.CurrentDirectory
            }
        };
        foreach (var arg in request.Arguments) process.StartInfo.ArgumentList.Add(arg);
        var output = new StringBuilder();
        var error = new StringBuilder();
        Task readOutput = Task.CompletedTask, readError = Task.CompletedTask;
        var started = false;
        try
        {
            lifetime.Token.ThrowIfCancellationRequested();
            started = process.Start();
            if (!started) return new(null, "", "Process failed to start.");
            readOutput = DrainAsync(process.StandardOutput, output, lifetime.Token);
            readError = DrainAsync(process.StandardError, error, lifetime.Token);
            await process.WaitForExitAsync(lifetime.Token).ConfigureAwait(false);
            await Task.WhenAll(readOutput, readError).ConfigureAwait(false);
            lifetime.Token.ThrowIfCancellationRequested();
            return new(process.ExitCode, output.ToString(), error.ToString());
        }
        catch (Exception exception) when (exception is OperationCanceledException or System.ComponentModel.Win32Exception or IOException or InvalidOperationException)
        {
            var canceled = lifetime.IsCancellationRequested;
            lifetime.Cancel();
            if (started)
            {
                // 只终止本次创建的进程树，不按名称结束用户已有的编辑器或客户端。
                try
                {
                    if (!process.HasExited) process.Kill(entireProcessTree: true);
                    await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                }
                catch (Exception cleanup) when (cleanup is InvalidOperationException or System.ComponentModel.Win32Exception or TimeoutException)
                {
                    exception = new AggregateException(exception, cleanup);
                }
            }
            await Task.WhenAll(readOutput, readError).ConfigureAwait(false);
            return new(null, output.ToString(), error + Environment.NewLine + exception.Message, canceled);
        }
    }

    private static async Task DrainAsync(StreamReader reader, StringBuilder tail, CancellationToken cancellationToken)
    {
        var buffer = new char[4096];
        try
        {
            int count;
            while ((count = await reader.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false)) > 0)
            {
                tail.Append(buffer, 0, count);
                if (tail.Length > MaxOutputChars) tail.Remove(0, tail.Length - MaxOutputChars);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (IOException) when (cancellationToken.IsCancellationRequested) { }
    }
}
