/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2008 by AG-Software 											 *
 * All Rights Reserved.																 *
 * Contact information for AG-Software is available at http://www.ag-software.de	 *
 *																					 *
 * Licence:																			 *
 * The agsXMPP SDK is released under a dual licence									 *
 * agsXMPP can be used under either of two licences									 *
 * 																					 *
 * A commercial licence which is probably the most appropriate for commercial 		 *
 * corporate use and closed source projects. 										 *
 *																					 *
 * The GNU Public License (GPL) is probably most appropriate for inclusion in		 *
 * other open source projects.														 *
 *																					 *
 * See README.html for details.														 *
 *																					 *
 * For general enquiries visit our website at:										 *
 * http://www.ag-software.de														 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

namespace ASC.Xmpp.Core.IO.Compression.Streams
{

    #region usings

    #endregion

    /// <summary>
    ///   Contains the output from the Inflation process. We need to have a window so that we can refer backwards into the output stream to repeat stuff. <br /> Author of the original java version : John Leuner
    /// </summary>
    public class OutputWindow
    {
        #region Members

        /// <summary>
        /// </summary>
        private static readonly int WINDOW_MASK = WINDOW_SIZE - 1;

        /// <summary>
        /// </summary>
        private static int WINDOW_SIZE = 1 << 15;

        /// <summary>
        /// </summary>
        private readonly byte[] window = new byte[WINDOW_SIZE]; // The window is 2^15 bytes

        /// <summary>
        /// </summary>
        private int windowEnd;

        /// <summary>
        /// </summary>
        private int windowFilled;

        #endregion

        #region Methods

        /// <summary>
        ///   Write a byte to this output window
        /// </summary>
        /// <param name="abyte"> value to write </param>
        /// <exception cref="InvalidOperationException">if window is full</exception>
        public void Write(int abyte)
        {
            if (windowFilled++ == WINDOW_SIZE)
            {
                throw new InvalidOperationException("Window full");
            }

            window[windowEnd++] = (byte) abyte;
            windowEnd &= WINDOW_MASK;
        }

        /// <summary>
        ///   Append a byte pattern already in the window itself
        /// </summary>
        /// <param name="len"> length of pattern to copy </param>
        /// <param name="dist"> distance from end of window pattern occurs </param>
        /// <exception cref="InvalidOperationException">If the repeated data overflows the window</exception>
        public void Repeat(int len, int dist)
        {
            if ((windowFilled += len) > WINDOW_SIZE)
            {
                throw new InvalidOperationException("Window full");
            }

            int rep_start = (windowEnd - dist) & WINDOW_MASK;
            int border = WINDOW_SIZE - len;
            if (rep_start <= border && windowEnd < border)
            {
                if (len <= dist)
                {
                    Array.Copy(window, rep_start, window, windowEnd, len);
                    windowEnd += len;
                }
                else
                {
                    /* We have to copy manually, since the repeat pattern overlaps. */
                    while (len-- > 0)
                    {
                        window[windowEnd++] = window[rep_start++];
                    }
                }
            }
            else
            {
                SlowRepeat(rep_start, len, dist);
            }
        }

        /// <summary>
        ///   Copy from input manipulator to internal window
        /// </summary>
        /// <param name="input"> source of data </param>
        /// <param name="len"> length of data to copy </param>
        /// <returns> the number of bytes copied </returns>
        public int CopyStored(StreamManipulator input, int len)
        {
            len = Math.Min(Math.Min(len, WINDOW_SIZE - windowFilled), input.AvailableBytes);
            int copied;

            int tailLen = WINDOW_SIZE - windowEnd;
            if (len > tailLen)
            {
                copied = input.CopyBytes(window, windowEnd, tailLen);
                if (copied == tailLen)
                {
                    copied += input.CopyBytes(window, 0, len - tailLen);
                }
            }
            else
            {
                copied = input.CopyBytes(window, windowEnd, len);
            }

            windowEnd = (windowEnd + copied) & WINDOW_MASK;
            windowFilled += copied;
            return copied;
        }

        /// <summary>
        ///   Copy dictionary to window
        /// </summary>
        /// <param name="dict"> source dictionary </param>
        /// <param name="offset"> offset of start in source dictionary </param>
        /// <param name="len"> length of dictionary </param>
        /// <exception cref="InvalidOperationException">If window isnt empty</exception>
        public void CopyDict(byte[] dict, int offset, int len)
        {
            if (windowFilled > 0)
            {
                throw new InvalidOperationException();
            }

            if (len > WINDOW_SIZE)
            {
                offset += len - WINDOW_SIZE;
                len = WINDOW_SIZE;
            }

            Array.Copy(dict, offset, window, 0, len);
            windowEnd = len & WINDOW_MASK;
        }

        /// <summary>
        ///   Get remaining unfilled space in window
        /// </summary>
        /// <returns> Number of bytes left in window </returns>
        public int GetFreeSpace()
        {
            return WINDOW_SIZE - windowFilled;
        }

        /// <summary>
        ///   Get bytes available for output in window
        /// </summary>
        /// <returns> Number of bytes filled </returns>
        public int GetAvailable()
        {
            return windowFilled;
        }

        /// <summary>
        ///   Copy contents of window to output
        /// </summary>
        /// <param name="output"> buffer to copy to </param>
        /// <param name="offset"> offset to start at </param>
        /// <param name="len"> number of bytes to count </param>
        /// <returns> The number of bytes copied </returns>
        /// <exception cref="InvalidOperationException">If a window underflow occurs</exception>
        public int CopyOutput(byte[] output, int offset, int len)
        {
            int copy_end = windowEnd;
            if (len > windowFilled)
            {
                len = windowFilled;
            }
            else
            {
                copy_end = (windowEnd - windowFilled + len) & WINDOW_MASK;
            }

            int copied = len;
            int tailLen = len - copy_end;

            if (tailLen > 0)
            {
                Array.Copy(window, WINDOW_SIZE - tailLen, output, offset, tailLen);
                offset += tailLen;
                len = copy_end;
            }

            Array.Copy(window, copy_end - len, output, offset, len);
            windowFilled -= copied;
            if (windowFilled < 0)
            {
                throw new InvalidOperationException();
            }

            return copied;
        }

        /// <summary>
        ///   Reset by clearing window so <see cref="GetAvailable">GetAvailable</see> returns 0
        /// </summary>
        public void Reset()
        {
            windowFilled = windowEnd = 0;
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// </summary>
        /// <param name="repStart"> </param>
        /// <param name="len"> </param>
        /// <param name="dist"> </param>
        private void SlowRepeat(int repStart, int len, int dist)
        {
            while (len-- > 0)
            {
                window[windowEnd++] = window[repStart++];
                windowEnd &= WINDOW_MASK;
                repStart &= WINDOW_MASK;
            }
        }

        #endregion
    }
}