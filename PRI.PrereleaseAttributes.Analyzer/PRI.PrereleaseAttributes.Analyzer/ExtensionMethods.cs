using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PRI.PrereleaseAttributes.Analyzer
{
	public static class ExtensionMethods
	{
		[NotNull]
		public static string[] AttributeFullNames(this SyntaxList<AttributeListSyntax> collection,
			SyntaxNodeAnalysisContext analysisContext)
		{
			var attributeSyntaxes = collection.SelectMany(e => e.Attributes);
			return attributeSyntaxes
				.Select(e => analysisContext.SemanticModel.GetTypeInfo(e).Type.ToString())
				.ToArray();
		}
	}
}