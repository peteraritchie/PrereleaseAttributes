using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PRI.PrereleaseAttributes.Analyzer.Test
{
	[TestClass]
	public class UtilityTests
	{

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DeclarationSyntaxWrapperWithUnsupportedTypeThrows()
		{
			var source = @"public class Test
		{
			x=1;
		}";
			SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
			var compilation = CSharpCompilation.Create("test", new[] { tree });
			var node = tree.GetRoot().ChildNodes().Single().ChildNodes().First();
			var sut = new DeclarationSyntaxFacade(compilation.GetSemanticModel(tree), node as MemberDeclarationSyntax);
		}
	}
}