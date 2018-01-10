﻿using PdfTextReader.Base;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class IdentifyTables : IProcessBlock
    {
        const float DARKCOLOR = 0.5f;

        private BlockPage _pageResult;
        private BlockPage _pageLines;
        private BlockPage _pageBackground;
        public BlockPage PageTables => _pageResult;
        public BlockPage PageLines=> _pageLines;
        public BlockPage PageBackground => _pageBackground;

        public void SetPageTables(IEnumerable<IBlock> tables)
        {
            var page = new BlockPage();
            page.AddRange(tables);

            if (HasTableOverlap(page))
                throw new InvalidOperationException();

            _pageResult = page;
        }

        public BlockPage Process(BlockPage page)
        {
            // try to improve processing time
            var cellList = page.AllBlocks.Where(b => ((TableCell)b).BgColor < DARKCOLOR).ToList();

            var blockArray = new TableSet[cellList.Count];
            
            bool hasModification = true;
            while (hasModification)
            {
                hasModification = false;

                // iterate every line found
                for (int i = 0; i < cellList.Count; i++)
                {
                    var c = cellList[i];

                    if (blockArray[i] == null)
                    {
                        // create a fresh blockset
                        blockArray[i] = new TableSet();
                        // add the current element to the blockset
                        blockArray[i].Add(c);
                    }

                    var currentBlockset = blockArray[i];

                    // assume that currentBlockset ALWAYS contains c
                    // -- it was added during blockArray assignment

                    // look for connected lines
                    for (int j = i + 1; j < cellList.Count; j++)
                    {
                        // skip if it already has block array assigned
                        if (blockArray[j] == currentBlockset)
                            continue;

                        var last = cellList[j];

                        // check if blockSet contains c (two rectangles)
                        float b_x1 = last.GetX();
                        float b_x2 = last.GetX() + last.GetWidth();
                        float b_y1 = last.GetH();
                        float b_y2 = last.GetH() + last.GetHeight();

                        var blockSet = currentBlockset;
                        bool b1 = HasOverlap(blockSet, b_x1, b_y1);
                        bool b2 = HasOverlap(blockSet, b_x1, b_y2);
                        bool b3 = HasOverlap(blockSet, b_x2, b_y2);
                        bool b4 = HasOverlap(blockSet, b_x2, b_y1);
                        
                        bool hasOverlap = b1 || b2 || b3 || b4;

                        // for some reason, hasOverlap is not 100% guarantee to work
                        if( blockArray[j] != null )
                        {
                            if (currentBlockset == null)
                                throw new InvalidOperationException();

                            bool bb = Block.HasOverlap(blockArray[j], currentBlockset);

                            if ((!hasOverlap) && bb)
                                hasOverlap = true;
                        }

                        // FOUND A CONNECTED LINE!
                        if (hasOverlap)
                        {
                            hasModification = true;

                            var nextBlockset = blockArray[j];


                            if (nextBlockset == null)
                            {
                                if (nextBlockset == currentBlockset)
                                    throw new InvalidOperationException("infinite loop?");

                                // assign the blockarray
                                blockArray[j] = currentBlockset;
                                // and add the element
                                blockArray[j].Add(last);
                            }
                            else
                            {
                                // has to merge changes
                                currentBlockset.MergeWith(nextBlockset);
                                // assign the blockarray
                                blockArray[j] = currentBlockset;
                                // assume nextBlockset already contains j

                                // remove all other references to nextBlockset
                                for (int k = 0; k < blockArray.Length; k++)
                                {
                                    if (blockArray[k] == nextBlockset)
                                        blockArray[k] = currentBlockset;
                                }
                            }
                        }
                        else
                        {
                            // do nothing
                        }
                    }
                }

                // infinite loop?
            }

            // transform blockArray into blockList
            var blockList = blockArray.Distinct().ToList();
            int count1 = blockArray.Length;
            int count2 = blockList.Count;

            var result = new BlockPage();
            var lines = new BlockPage();
            var background = new BlockPage();
            
            foreach (var b in blockList)
            {
                // does not add line segments
                if (b.Count() == 1)
                    lines.Add(b);
                else
                    result.Add(b);
            }

            // add background
            var dark = page.AllBlocks
                        .Where(b => ((TableCell)b).BgColor >= DARKCOLOR)
                        .Select(b => new TableSet() { b });

            background.AddRange(dark);

            this._pageResult = result;
            this._pageLines = lines;
            this._pageBackground = background;

            if(HasTableOverlap(result))
            {
                throw new InvalidOperationException("cannot have overlapped table");
            }

            return result;
        }

        bool HasTableOverlap(BlockPage page)
        {
            foreach(var a in page.AllBlocks)
            {
                foreach (var b in page.AllBlocks)
                {
                    if (a == b)
                        continue;

                    if (Block.HasOverlap(a, b))
                        return true;
                }
            }
            return false;
        }

        static bool HasOverlap(IBlock blockSet, float x, float h)
        {
            float a_x1 = blockSet.GetX();
            float a_x2 = blockSet.GetX() + blockSet.GetWidth();
            float a_y1 = blockSet.GetH();
            float a_y2 = blockSet.GetH() + blockSet.GetHeight();

            bool hasOverlap = ((a_x1 <= x) && (a_x2 >= x) && (a_y1 <= h) && (a_y2 >= h));

            return hasOverlap;
        }        
    }
}
