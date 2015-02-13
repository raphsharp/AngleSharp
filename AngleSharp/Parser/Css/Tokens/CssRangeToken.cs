﻿namespace AngleSharp.Parser.Css
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using AngleSharp.Core;

    /// <summary>
    /// Represents the CSS range token.
    /// </summary>
    sealed class CssRangeToken : CssToken
    {
        #region Fields

        readonly String[] _range;

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new CSS range token.
        /// </summary>
        /// <param name="start">The (hex-)string where to begin.</param>
        /// <param name="end">The (hex-)string where to end.</param>
        public CssRangeToken(String start, String end)
            : base(CssTokenType.Range, String.Empty)
        {
            var index = Int32.Parse(start, NumberStyles.HexNumber);

            if (index <= Symbols.MaximumCodepoint)
            {
                if (end != null)
                {
                    var list = new List<String>();
                    var f = Int32.Parse(end, NumberStyles.HexNumber);

                    if (f > Symbols.MaximumCodepoint)
                        f = Symbols.MaximumCodepoint;

                    for (; index <= f; index++)
                        list.Add(index.ConvertFromUtf32());

                    _range = list.ToArray();
                }
                else
                    _range = new String[] { index.ConvertFromUtf32() };
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the status of the range.
        /// </summary>
        public Boolean IsEmpty
        {
            get { return _range == null || _range.Length == 0; }
        }

        /// <summary>
        /// Gets the content of the range token.
        /// </summary>
        public String[] SelectedRange
        {
            get { return _range; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string which represents the original value.
        /// </summary>
        /// <returns>The original value.</returns>
        public override String ToValue()
        {
            if (IsEmpty)
                return String.Empty; 
            if (_range.Length == 1)
                return "#" + PlatformExtensions.ConvertToUtf32(_range[0], 0).ToString("x");
            return "#" + PlatformExtensions.ConvertToUtf32(_range[0], 0).ToString("x") + "-#" + PlatformExtensions.ConvertToUtf32(_range[_range.Length - 1], 0).ToString("x");
        }

        #endregion
    }
}
