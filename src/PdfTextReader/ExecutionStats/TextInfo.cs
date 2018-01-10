﻿using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.ExecutionStats
{
    public class TextInfo
    {
        public string FontName;
        public decimal FontSize;
        public string FontStyle;
        public string Text;

        public TextInfo(TextLine line)
        {
            this.FontName = line.FontName;
            this.FontSize = Decimal.Round(Convert.ToDecimal(line.FontSize),2);
            this.FontStyle = line.FontStyle;
            this.Text = line.Text;
        }

        public TextInfo(string fontName, string fontStyle, decimal fontSize)
        {
            this.FontName = fontName;
            this.FontStyle = fontStyle;
            this.FontSize = fontSize;
            this.Text = "";
        }
    }
}
