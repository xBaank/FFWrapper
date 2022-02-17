﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using FFmpegWrapper.Extensions;
using FFmpegWrapper.JsonModels;

namespace FFmpegWrapper.Models
{
    /// <summary>
    /// FFmpeg process, use FFmpegProcessBuilder to create a FFmpegProcess or FFmpegClient to convert mediaFiles
    /// </summary>
    public class FFProcess : Process
    {


        public new event Action<FFProcess, string?>? ErrorDataReceived;
        public new event Action<FFProcess, byte[]>? OutputDataReceived;
        public event Action<FFProcess>? ExitedWithError;

        internal Stream? Input { get; set; }
        internal Stream? Output { get; set; }
        internal string? Error { get; set; }
        internal int InputBuffer { get; set; } = 4096;
        internal int OutputBuffer { get; set; } = 4096;

        private List<Task> tasks = new List<Task>();


        internal FFProcess()
        {
            //Don't allow end user to create process directly
        }

        public new void Start() => StartProcess().tasks.WaitAll();
        public Task StartAsync() => StartProcess().tasks.WhenAll();

        public async Task<T?> DeserializeResultAsync<T>()
        {
            if (Output is null || ExitCode != 0)
                return default(T);

            Output.Seek(0, SeekOrigin.Begin);

            var result = await JsonSerializer.DeserializeAsync<T?>(Output);
            return result;
        }

        private Task PipeInput()
        {
            if (Input == null)
                throw new NullReferenceException("Input set to null");

            return Task.Run(async () =>
            {
                byte[] bytes = new byte[InputBuffer];
                int bytesRead;

                while ((bytesRead = await Input.ReadAsync(bytes, 0, bytes.Length)) != 0)
                    await StandardInput.BaseStream.WriteAsync(bytes, 0, bytesRead);

                StandardInput.Close();
            });
        }

        private Task PipeOutput()
        {

            return Task.Run(async () =>
            {
                byte[] bytes = new byte[OutputBuffer];
                int bytesRead;

                while ((bytesRead = await StandardOutput.BaseStream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                {
                    if (Output != null)
                        await Output.WriteAsync(bytes, 0, bytesRead);

                    CallOutputEvent(bytes);
                }

            });
        }

        private Task PipeError()
        {
            return Task.Run(async () =>
            {
                while (!StandardError.EndOfStream)
                {
                    var line = await StandardError.ReadLineAsync();
                    CallErrorEvent(line);
                    Error += line;

                }
            });
        }

        public async Task<byte[]> GetNextBytes()
        {
            if (!StartInfo.RedirectStandardOutput)
                throw new InvalidOperationException("Output not being redirected");

            if (Output is not null)
                throw new InvalidOperationException("All output data is being written to the output stream");

            byte[] bytes = new byte[OutputBuffer];
            int bytesRead = await StandardOutput.BaseStream.ReadAsync(bytes, 0, OutputBuffer);

            bytes = bytes.Take(bytesRead).ToArray();
            CallOutputEvent(bytes);
            return bytes;
        }

        private FFProcess StartProcess()
        {
            Exited += new EventHandler(CallExitEvent);
            base.Start();

            if (StartInfo.RedirectStandardInput)
                tasks.Add(PipeInput());

            if (StartInfo.RedirectStandardOutput && Output is not null)
                tasks.Add(PipeOutput());

            if (StartInfo.RedirectStandardError)
                tasks.Add(PipeError());


            return this;
        }

        private void CallOutputEvent(byte[] bytes) => OutputDataReceived?.Invoke(this, bytes);
        private void CallErrorEvent(string? messageException) => ErrorDataReceived?.Invoke(this, messageException);
        private void CallExitEvent(object? sender, EventArgs e)
        {
            Process? process = (Process?)sender;

            if (process?.ExitCode != 0)
                ExitedWithError?.Invoke(this);
        }
    }
}