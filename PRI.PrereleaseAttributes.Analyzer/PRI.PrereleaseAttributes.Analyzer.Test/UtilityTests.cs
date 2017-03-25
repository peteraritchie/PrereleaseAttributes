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
			public object this[int index]
			{
				get { return null; }
				set { }
			}
		}";
			SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
			var compilation = CSharpCompilation.Create("test", new[] { tree });
			var node = tree.GetRoot().ChildNodes().Single().ChildNodes().First();
			var sut = new DeclarationSyntaxWrapper(compilation.GetSemanticModel(tree), node as MemberDeclarationSyntax);
		}
	}
}