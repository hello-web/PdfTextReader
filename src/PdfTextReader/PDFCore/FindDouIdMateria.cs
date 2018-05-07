﻿using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class FindDouIdMateria : IProcessBlock
    {
        const float CONSIDERED_VERY_SMALL_FONTSIZE = 0.3f;
        const float SAME_LINE_SMALL_FONTSIZE = 0.1f;

        public BlockPage Process(BlockPage page)
        {        
            var result = new BlockPage();

            Block last_box = null;

            foreach (var block in page.AllBlocks)
            {
                if (((Block)block).FontSize <= CONSIDERED_VERY_SMALL_FONTSIZE)
                {
                    float boxSize = 8f;

                    var box = new BlockHidden()
                    {
                        X = block.GetX() - boxSize,
                        H = block.GetH() - boxSize,
                        Width = block.GetWidth() + 2*boxSize,
                        Height = block.GetHeight() + 2*boxSize,
                        Text = block.GetText()
                    };

                    if( last_box != null )
                    {
                        float lastH = last_box.GetH();
                        float curH = box.GetH();

                        // sometimes the block is broken.. merge them
                        if( Math.Abs(lastH - curH) < SAME_LINE_SMALL_FONTSIZE )
                        {
                            // we dont expect to have last after the current
                            if (last_box.GetX() > box.GetX())
                                PdfReaderException.AlwaysThrow("last_box.GetX() > box.GetX()");

                            last_box.Text += box.GetText();
                            box.Text = "";
                        }
                    }

                    result.Add(box);

                    if (box.Text != "")
                    {
                        last_box = box;
                    }
                }
                else
                {
                    result.Add(block);
                }
            }

            return result;
        }
    }
}
