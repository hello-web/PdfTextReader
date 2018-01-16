﻿using PdfTextReader.Base;
using PdfTextReader.Execution;
using PdfTextReader.ExecutionStats;
using PdfTextReader.Parser;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader
{
    class ExamplesWork
    {
        public static void Blocks(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-01-blocks-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                    .Show(Color.Orange);

            pipeline.Done();
        }

        public static void BlockLines(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-02-blockline-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<GroupLines>()
                    .Show(Color.Red);

            pipeline.Done();
        }
        public static void BlockSets(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-03-blocksets-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                    .Show(Color.Orange);

            pipeline.Done();
        }
    }
}
