using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace PRI.ReleaseAttributes.Analyzer.Test
{
	[TestClass]
	public class UnitTest : CodeFixVerifier
	{
		[TestMethod]
		public void PrereleaseMethodReturnInPrereleaseTypeDoesNotCauseDiagnostic()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType { static OtherType CreateNew() { return new OtherType(); } }
	
		class TypeName
		{
			public void Method()
			{
			}
		}
	}";

			#endregion test-code

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void PrereleasePropertyTypeInPrereleaseTypeDoesNotCauseDiagnostic()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType
		{
			[Prerelease]
			public int Number {get;set;}
		}
	
		class TypeName
		{
			public void Method()
			{
			}
		}
	}";
			#endregion test-code

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void PrereleaseFieldTypeInPrereleaseTypeDoesNotCauseDiagnostic()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public class OtherType { public static OtherType Instance = new OtherType();}}
	
		class TypeName
		{
			OtherType o = OtherType.Instance;
			public void Method()
			{
			}
		}
	}";
			#endregion test-code

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void PrereleaseParameterTypeInPrereleaseTypeDoesNotCauseDiagnostic()
		{
			{
				#region test-code
				var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType
		{
			public int Number {get;set;}
			public void DoSomething(OtherType o) {}
		}
	
		class TypeName
		{
			public void Method()
			{
			}
		}
	}";
				#endregion test-code

				VerifyCSharpDiagnostic(test);
			}
		}

		//No diagnostics expected to show up
		[TestMethod]
		public void TestMethod1()
		{
			var test = @"";

			VerifyCSharpDiagnostic(test);
		}

		//Diagnostic and CodeFix both triggered and checked for
		[TestMethod]
		public void TestDeclaredVariablePrereleaseTypeWithAssignment()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType { static int DidIt(){return true;}}
	
		class TypeName
		{
			public void Method()
			{
				var o = new OtherType();
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0100",
				Message = "Variable name 'o' instantiates prerelease type 'ConsoleApplication1.OtherType'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 19, column: 9)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredVariablePrereleaseType()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType { static int DidIt(){return true;}}
	
		class TypeName
		{
			public void Method()
			{
				OtherType otherType;
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0100",
				Message = "Variable name 'otherType' instantiates prerelease type 'ConsoleApplication1.OtherType'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 19, column: 15)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredVariableTypeInPrereleaseAssembly()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	[assembly: Prerelease]
	namespace ConsoleApplication1
	{
		public static class OtherType { static int DidIt(){return true;}}
	
		class TypeName
		{
			public void Method()
			{
				OtherType otherType;
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0102",
				Message = "Variable name 'otherType' instantiates type 'ConsoleApplication1.OtherType' in prerelease assembly",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 19, column: 15)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredFieldPrereleaseTypeWithAssignmentFromNew()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType { static int DidIt(){return true;}}
	
		class TypeName
		{
			OtherType o = new OtherType()
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0100",
				Message = "Field name 'o' instantiates prerelease type 'ConsoleApplication1.OtherType'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 17, column: 14)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredFieldPrereleaseTypeWithAssignmentFromMethodFromPrereleaseAssembly()
		{
			#region test-code
			var source1 = @"using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	[assembly: Prerelease]
	namespace ClassLibrary
	{
		public static class OtherType { static OtherType CreateNew() {return new OtherType();}}
	}";
			var source2 = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using ClassLibrary;

	namespace ConsoleApplication1
	{
		class TypeName
		{
			OtherType o = OtherType.CreateNew()
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0102",
				Message = "Field name 'o' instantiates type 'ClassLibrary.OtherType' in prerelease assembly",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test1.cs", line: 14, column: 14)
						}
			};

			VerifyCSharpDiagnostic(expected, source1, source2);
		}

		[TestMethod]
		public void TestDeclaredFieldPrereleaseTypeWithAssignmentFromPropertyFromPrereleaseAssembly()
		{
			#region test-code
			var source1 = @"using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	[assembly: Prerelease]
	namespace ClassLibrary
	{
		public static class OtherType { static OtherType CreateNew() {return new OtherType();}}
	}";
			var source2 = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using ClassLibrary;

	namespace ConsoleApplication1
	{
		class TypeName
		{
			OtherType o = OtherType.CreateNew()
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0102",
				Message = "Field name 'o' instantiates type 'ClassLibrary.OtherType' in prerelease assembly",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test1.cs", line: 14, column: 14)
						}
			};

			VerifyCSharpDiagnostic(expected, source1, source2);
		}

		[TestMethod]
		public void TestDeclaredFieldPrereleaseTypeWithAssignmentFromMethod()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType { static OtherType CreateNew() {return new OtherType();}}
	
		class TypeName
		{
			OtherType o = OtherType.CreateNew()
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0100",
				Message = "Field name 'o' instantiates prerelease type 'ConsoleApplication1.OtherType'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 17, column: 14)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredFieldTypeWithAssignmentFromPropertyInPrereleaseType()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		public class OtherType
		{
			[Prerelease]
			static OtherType Instance { get { return new OtherType(); } }
		}
	
		class TypeName
		{
			OtherType o = OtherType.Instance;
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0101",
				Message = "Field name 'o' uses prerelease member 'ConsoleApplication1.OtherType.Instance'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 20, column: 14)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredFieldTypeWithAssignmentFromPrereleaseProperty()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		public class OtherType
		{
			[Prerelease]
			static OtherType Instance { get { return new OtherType(); } }
		}
	
		class TypeName
		{
			OtherType o = OtherType.Instance;
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0101",
				Message = "Field name 'o' uses prerelease member 'ConsoleApplication1.OtherType.Instance'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 20, column: 14)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredFieldTypeWithAssignmentFromPrereleaseField()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public class OtherType { public static OtherType Instance = new OtherType();}
	
		class TypeName
		{
			OtherType o = OtherType.Instance;
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0100",
				Message = "Field name 'o' instantiates prerelease type 'ConsoleApplication1.OtherType'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 17, column: 14)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredFieldPrereleaseType()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType { static int DidIt(){return true;}}
	
		class TypeName
		{
			OtherType otherType;
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0100",
				// TODO: "uses" not "instantiates"
				Message = "Field name 'otherType' instantiates prerelease type 'ConsoleApplication1.OtherType'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 17, column: 14)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredFieldPrereleaseInitializer()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType { static int DidIt(){return true;}}
	
		class TypeName
		{
			private Object otherType = new OtherType();
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0100",
				Message = "Field name 'otherType' instantiates prerelease type 'ConsoleApplication1.OtherType'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 17, column: 19)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredFieldPrereleaseAssemblyInitializer()
		{
			#region test-code

			var source1 = @"using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	[assembly: Prerelease]
	namespace ClassLibrary
	{
		public static class OtherType { static int DidIt(){return true;}}
	}";
			var source2 = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using ClassLibrary;

	namespace ConsoleApplication1
	{
		class TypeName
		{
			private Object otherType = new OtherType();
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0102",
				Message = "Field name 'otherType' instantiates type 'ClassLibrary.OtherType' in prerelease assembly",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test1.cs", line: 14, column: 19)
						}
			};

			VerifyCSharpDiagnostic(expected, source1, source2);
		}

		private void VerifyCSharpDiagnostic(DiagnosticResult expected, params string[] sources)
		{
			IEnumerable<Project> projects;
			IEnumerable<Document> documents;
			var analyzer = CreateCSharpProjects(sources, out projects, out documents);
			var results = new List<Diagnostic>();
			foreach (var project in projects)
			{
				var compilationWithAnalyzers = project.GetCompilationAsync().Result.WithAnalyzers(ImmutableArray.Create(analyzer));
				var diags = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;
				foreach (var diag in diags)
				{
					if (diag.Location == Location.None || diag.Location.IsInMetadata)
					{
						results.Add(diag);
					}
					else
					{
						for (int i = 0; i < documents.Count(); i++)
						{
							var document = documents.ElementAt(i);
							var tree = document.GetSyntaxTreeAsync().Result;
							if (tree == diag.Location.SourceTree)
							{
								results.Add(diag);
							}
						}
					}
				}
			}

			VerifyDiagnosticResults(SortDiagnostics(results), analyzer, expected);
		}

		private DiagnosticAnalyzer CreateCSharpProjects(string[] sources, out IEnumerable<Project> projects, out IEnumerable<Document> documents)
		{
			var analyzer = GetCSharpDiagnosticAnalyzer();
			projects = GetProjects(sources, LanguageNames.CSharp);
			documents = projects.SelectMany(e => e.Documents);
			return analyzer;
		}

		[TestMethod]
		public void TestDeclaredFieldTypeInPrereleaseAssembly()
		{
			#region test-code
			var source1 = @"using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	[assembly: Prerelease]
	namespace ClassLibrary
	{
		public static class OtherType { static int DidIt(){return true;}}
	}";

			var source2 = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using ClassLibrary;

	namespace ConsoleApplication1
	{
		class TypeName
		{
			private OtherType otherType;
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0102",
				// TODO: "uses", not "instantiates"
				Message = "Field name 'otherType' instantiates type 'ClassLibrary.OtherType' in prerelease assembly",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test1.cs", line: 14, column: 22)
						}
			};

			VerifyCSharpDiagnostic(expected, source1, source2);
		}

		[TestMethod]
		public void TestDeclaredPropertyPrereleaseType()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType { static int DidIt(){return true;}}
	
		class TypeName
		{
			OtherType otherType {get;set};
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0100",
				Message = "Property name 'otherType' instantiates prerelease type 'ConsoleApplication1.OtherType'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 17, column: 14)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredPropertyTypeInPrereleaseAssembly()
		{
			#region test-code
			var source1 = @"using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	[assembly: Prerelease]
	namespace ClassLibrary
	{
		public static class OtherType { static int DidIt(){return true;}}
	}";
			var source2 = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using ClassLibrary;

	namespace ConsoleApplication1
	{
		class TypeName
		{
			OtherType otherType {get;set};
			public void Method()
			{
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0102",
				Message = "Property name 'otherType' instantiates type 'ClassLibrary.OtherType' in prerelease assembly",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test1.cs", line: 14, column: 14)
						}
			};

			VerifyCSharpDiagnostic(expected, source1, source2);
		}

		[TestMethod]
		public void TestDeclaredMethodReturnTypePrereleaseType()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType { static int DidIt(){return true;}}
	
		class TypeName
		{
			public OtherType Method()
			{
				throw new NotImplementedException();
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0103",
				Message = "Method name 'Method' returns prerelease type 'ConsoleApplication1.OtherType'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 17, column: 21)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredMethodReturnTypePrereleaseAssemblyType()
		{
			#region test-code
			var source1 = @"using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	[assembly: Prerelease]
	namespace ClassLibrary
	{
		public static class OtherType { static int DidIt(){return true;}}
	}";
			var source2 = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using ClassLibrary;

	namespace ConsoleApplication1
	{
		class TypeName
		{
			public OtherType Method()
			{
				throw new NotImplementedException();
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0104",
				Message = "Method name 'Method' returns type 'ClassLibrary.OtherType' in prerelease assembly",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test1.cs", line: 14, column: 21)
						}
			};

			VerifyCSharpDiagnostic(expected, source1, source2);
		}

		[TestMethod]
		public void TestDeclaredMethodParametersPrereleaseType()
		{
			#region test-code
			var test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	namespace ConsoleApplication1
	{
		[Prerelease]
		public static class OtherType { static int DidIt(){return true;}}
	
		class TypeName
		{
			public void Method(OtherType otherType)
			{
				throw new NotImplementedException();
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0100",
				Message = "Parameter name 'otherType' instantiates prerelease type 'ConsoleApplication1.OtherType'",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line: 17, column: 33)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestDeclaredMethodParametersPrereleaseAssemblyType()
		{
			#region test-code
			var source1 = @"using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using PRI.PrereleaseAttributes;

	[assembly: Prerelease]
	namespace ClassLibrary
	{
		public static class OtherType { static int DidIt(){return true;}}
	}";
			var source2 = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using ClassLibrary;

	namespace ConsoleApplication1
	{
		class TypeName
		{
			public void Method(OtherType otherType)
			{
				throw new NotImplementedException();
			}
		}
	}";
			#endregion test-code
			var expected = new DiagnosticResult
			{
				Id = "EA0102",
				Message = "Parameter name 'otherType' instantiates type 'ClassLibrary.OtherType' in prerelease assembly",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test1.cs", line: 14, column: 33)
						}
			};

			VerifyCSharpDiagnostic(expected, source1, source2);
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new PRIReleaseAttributesAnalyzerAnalyzer();
		}
	}
}