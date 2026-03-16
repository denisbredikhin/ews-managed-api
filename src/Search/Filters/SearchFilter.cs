/*
 * Exchange Web Services Managed API
 *
 * Copyright (c) Microsoft Corporation
 * All rights reserved.
 *
 * MIT License
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this
 * software and associated documentation files (the "Software"), to deal in the Software
 * without restriction, including without limitation the rights to use, copy, modify, merge,
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
 * to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */

namespace Microsoft.Exchange.WebServices.Data;
/// <summary>
/// Represents the base search filter class. Use descendant search filter classes such as SearchFilter.IsEqualTo,
/// SearchFilter.ContainsSubstring and SearchFilter.SearchFilterCollection to define search filters.
/// </summary>
public abstract partial class SearchFilter : ComplexProperty
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchFilter"/> class.
    /// </summary>
    internal SearchFilter()
    {
    }

    /// <summary>
    /// Loads from XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>SearchFilter.</returns>
    internal static SearchFilter LoadFromXml(EwsServiceXmlReader reader)
    {
        reader.EnsureCurrentNodeIsStartElement();

        string localName = reader.LocalName;

        SearchFilter searchFilter = GetSearchFilterInstance(localName);

        searchFilter?.LoadFromXml(reader, reader.LocalName);

        return searchFilter;
    }

    /// <summary>
    /// Gets the search filter instance.
    /// </summary>
    /// <param name="localName">Name of the local.</param>
    /// <returns></returns>
    private static SearchFilter GetSearchFilterInstance(string localName)
    {
        SearchFilter searchFilter = localName switch
        {
            XmlElementNames.Exists => new Exists(),
            XmlElementNames.Contains => new ContainsSubstring(),
            XmlElementNames.Excludes => new ExcludesBitmask(),
            XmlElementNames.Not => new Not(),
            XmlElementNames.And => new SearchFilterCollection(LogicalOperator.And),
            XmlElementNames.Or => new SearchFilterCollection(LogicalOperator.Or),
            XmlElementNames.IsEqualTo => new IsEqualTo(),
            XmlElementNames.IsNotEqualTo => new IsNotEqualTo(),
            XmlElementNames.IsGreaterThan => new IsGreaterThan(),
            XmlElementNames.IsGreaterThanOrEqualTo => new IsGreaterThanOrEqualTo(),
            XmlElementNames.IsLessThan => new IsLessThan(),
            XmlElementNames.IsLessThanOrEqualTo => new IsLessThanOrEqualTo(),
            _ => null,
        };
        return searchFilter;
    }

    /// <summary>
    /// Gets the name of the XML element.
    /// </summary>
    /// <returns>XML element name.</returns>
    internal abstract string GetXmlElementName();

    /// <summary>
    /// Writes to XML.
    /// </summary>
    /// <param name="writer">The writer.</param>
    internal virtual void WriteToXml(EwsServiceXmlWriter writer)
    {
        base.WriteToXml(writer, this.GetXmlElementName());
    }
}