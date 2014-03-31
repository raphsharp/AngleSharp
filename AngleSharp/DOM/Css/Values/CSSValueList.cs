﻿namespace AngleSharp.DOM.Css
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a list of values in the CSS context.
    /// </summary>
    sealed class CSSValueList : CSSValue
    {
        #region Fields

        readonly List<CSSValue> _items;
        CssValueListSeparator _separator;

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new CSS value list.
        /// </summary>
        internal CSSValueList()
        {
            _items = new List<CSSValue>();
            _type = CssValueType.ValueList;
            _separator = CssValueListSeparator.Space;
        }

        /// <summary>
        /// Creates a new CSS value list.
        /// </summary>
        /// <param name="item">The first item to add.</param>
        internal CSSValueList(CSSValue item)
			: this()
        {
			_items.Add(item);
        }

		#endregion

		#region Internal Properties

        /// <summary>
        /// Gets or sets the separator to use.
        /// </summary>
        internal CssValueListSeparator Separator
        {
            get { return _separator; }
            set { _separator = value; }
        }

		/// <summary>
		/// Gets the list with values.
		/// </summary>
		internal List<CSSValue> List
		{
			get { return _items; }
		}

		#endregion

        #region Properties

        /// <summary>
        /// Gets the number of CSSValues in the list.
        /// </summary>
        public Int32 Length
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Used to retrieve a CSSValue by ordinal index. The order in this collection represents the order of the values in the CSS style property.
        /// </summary>
        /// <param name="index">If index is greater than or equal to the number of values in the list, this returns null.</param>
        /// <returns>The value at the given index or null.</returns>
        [IndexerName("ListItems")]
        public CSSValue this[Int32 index]
        {
            get { return index >= 0 && index < _items.Count ? _items[index] : null; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Used to retrieve a CSSValue by ordinal index. The order in this collection represents the order of the values in the CSS style property.
        /// </summary>
        /// <param name="index">If index is greater than or equal to the number of values in the list, this returns null.</param>
        /// <returns>The value at the given index or null.</returns>
        public CSSValue Item(Int32 index)
        {
            return this[index];
        }

        /// <summary>
        /// Returns a CSS code representation of the stylesheet.
        /// </summary>
        /// <returns>A string that contains the code.</returns>
        public override String ToCss()
        {
            var values = new String[_items.Count];

            for (int i = 0; i < _items.Count; i++)
                values[i] = _items[i].CssText;

            return String.Join(_separator == CssValueListSeparator.Comma ? ", "  : (_separator == CssValueListSeparator.Slash ? " / " : " "), values);
        }

        #endregion

        #region Internal Methods

        internal CSSLengthValue ToLength(Int32 index, Boolean required = true)
        {
            if (_items.Count > index)
            {
                if (_items[index] is CSSLengthValue)
                    return (CSSLengthValue)_items[index];
                else if (_items[index] == CSSNumberValue.Zero)
                    required = false;
            }

            if (required)
                return null;

            return new CSSLengthValue(AngleSharp.Length.Zero);
        }

        internal CSSColorValue ToColor(Int32 index, Boolean required = true)
        {
            if (_items.Count > index)
            {
                if (_items[index] is CSSColorValue)
                    return (CSSColorValue)_items[index];
            }

            if (required)
                return null;

            return new CSSColorValue(AngleSharp.Color.Black);
        }

        #endregion
    }
}
