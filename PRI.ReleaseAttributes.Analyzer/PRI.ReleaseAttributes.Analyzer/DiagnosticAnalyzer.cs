using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using PRI.PrereleaseAttributes;

namespace PRI.ReleaseAttributes.Analyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class PRIReleaseAttributesAnalyzerAnalyzer : DiagnosticAnalyzer
	{
		#region Rules
		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
		private static readonly LocalizableString UseOfPrereleaseTypeTitle = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseTypeAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString UseOfPrereleaseTypeMessageFormat = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseTypeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString UseOfPrereleaseTypeDescription = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseTypeAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string UseOfPrereleaseTypeCategory = "Usage";

		private static readonly DiagnosticDescriptor UseOfPrereleaseTypeRule =
			new DiagnosticDescriptor("EA0100", UseOfPrereleaseTypeTitle,
				UseOfPrereleaseTypeMessageFormat, UseOfPrereleaseTypeCategory, DiagnosticSeverity.Warning,
				isEnabledByDefault: true, description: UseOfPrereleaseTypeDescription);

		private static readonly LocalizableString UseOfPrereleaseMemberTitle = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseMemberAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString UseOfPrereleaseMemberMessageFormat = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseMemberAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString UseOfPrereleaseMemberDescription = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseMemberAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string UseOfPrereleaseMemberCategory = "Usage";

		private static readonly DiagnosticDescriptor UseOfPrereleaseMemberRule =
			new DiagnosticDescriptor("EA0101", UseOfPrereleaseMemberTitle,
				UseOfPrereleaseMemberMessageFormat, UseOfPrereleaseMemberCategory, DiagnosticSeverity.Warning,
				isEnabledByDefault: true, description: UseOfPrereleaseMemberDescription);

		private static readonly LocalizableString UseOfPrereleaseAssemblyTitle = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseAssemblyAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString UseOfPrereleaseAssemblyMessageFormat = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseAssemblyAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString UseOfPrereleaseAssemblyDescription = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseAssemblyAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string UseOfPrereleaseAssemblyCategory = "Usage";

		private static readonly DiagnosticDescriptor UseOfPrereleaseAssemblyRule =
			new DiagnosticDescriptor("EA0102", UseOfPrereleaseAssemblyTitle,
				UseOfPrereleaseAssemblyMessageFormat, UseOfPrereleaseAssemblyCategory, DiagnosticSeverity.Warning,
				isEnabledByDefault: true, description: UseOfPrereleaseAssemblyDescription);

		private static readonly LocalizableString UseOfPrereleaseReturnTypeTitle = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseReturnTypeAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString UseOfPrereleaseReturnTypeMessageFormat = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseReturnTypeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString UseOfPrereleaseReturnTypeDescription = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseReturnTypeAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string UseOfPrereleaseReturnTypeCategory = "Usage";

		private static readonly DiagnosticDescriptor UseOfPrereleaseReturnTypeRule =
			new DiagnosticDescriptor("EA0103", UseOfPrereleaseReturnTypeTitle,
				UseOfPrereleaseReturnTypeMessageFormat, UseOfPrereleaseReturnTypeCategory, DiagnosticSeverity.Warning,
				isEnabledByDefault: true, description: UseOfPrereleaseReturnTypeDescription);

		private static readonly LocalizableString UseOfPrereleaseAssemblyReturnTypeTitle = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseAssemblyReturnTypeAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString UseOfPrereleaseAssemblyReturnTypeMessageFormat = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseAssemblyReturnTypeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString UseOfPrereleaseAssemblyReturnTypeDescription = new LocalizableResourceString(nameof(Resources.UseOfPrereleaseAssemblyReturnTypeAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string UseOfPrereleaseAssemblyReturnTypeCategory = "Usage";

		private static readonly DiagnosticDescriptor UseOfPrereleaseAssemblyReturnTypeRule =
			new DiagnosticDescriptor("EA0104", UseOfPrereleaseAssemblyReturnTypeTitle,
				UseOfPrereleaseAssemblyReturnTypeMessageFormat, UseOfPrereleaseAssemblyReturnTypeCategory, DiagnosticSeverity.Warning,
				isEnabledByDefault: true, description: UseOfPrereleaseAssemblyReturnTypeDescription);
		#endregion Rules

		private readonly string[] _prereleaseTypeNames = {
			typeof(PrereleaseAttribute).FullName,
			typeof(AlphaAttribute).FullName,
			typeof(ExperimentalAttribute).FullName,
			typeof(PreviewAttribute).FullName,
		};

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
			ImmutableArray.Create(UseOfPrereleaseTypeRule, UseOfPrereleaseMemberRule, UseOfPrereleaseAssemblyRule,
				UseOfPrereleaseReturnTypeRule, UseOfPrereleaseAssemblyReturnTypeRule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
			context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
			context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
		}

		private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext analysisContext)
		{
			var propertyDeclaration = new DeclarationSyntaxWrapper(analysisContext.SemanticModel,
				(PropertyDeclarationSyntax) analysisContext.Node);

			const string identifierContext = "Property";

			var identifier = propertyDeclaration.Identifier;
			var assemblyRule = UseOfPrereleaseAssemblyRule; // TODO:
			var typeRule = UseOfPrereleaseTypeRule; // TODO:

			TryReportPrereleaseAttributeDiagnostics(analysisContext, propertyDeclaration, identifier, propertyDeclaration.Type,
				typeRule, assemblyRule, identifierContext);
		}

		private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext analysisContext)
		{
			var fieldDeclaration = new DeclarationSyntaxWrapper(analysisContext.SemanticModel,
				(FieldDeclarationSyntax) analysisContext.Node);

			const string identifierContext = "Field";

			foreach (var node in fieldDeclaration.DescendantNodes())
			{
				var variableDeclarationSyntax = node as VariableDeclarationSyntax;
				if (variableDeclarationSyntax?.Variables.Count != 1)
				{
					continue;
				}
				var variable = variableDeclarationSyntax.Variables.Single();

				var assemblyRule = UseOfPrereleaseAssemblyRule;
				var typeRule = UseOfPrereleaseTypeRule;
				var identifier = variable.Identifier;
				// check field type
				if (TryReportPrereleaseAttributeDiagnostics(analysisContext, fieldDeclaration, identifier, fieldDeclaration.Type, typeRule, assemblyRule, identifierContext))
				{
					return;
				}
				var objectCreationSyntax = variable.Initializer.Value as ObjectCreationExpressionSyntax;
				
				// check the initializer from object creation:
				if (objectCreationSyntax?.Type != null)
				{
					var ti = analysisContext.SemanticModel.GetTypeInfo(variable.Initializer.Value);
					if (TryReportPrereleaseAttributeDiagnostics(analysisContext, fieldDeclaration, identifier, ti.Type, typeRule, assemblyRule, identifierContext))
					{
						return;
					}
				}

				// check the initialize from member access
				var memberAccessSyntax = variable.Initializer.Value as MemberAccessExpressionSyntax;
				if(memberAccessSyntax != null)
				{
					var symbolInfo = analysisContext.SemanticModel.GetSymbolInfo(memberAccessSyntax);
					var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();
					if(symbol != null)
					{
						TryReportPrereleaseAttributeDiagnostics(analysisContext, symbol, identifier, identifierContext);
					}
				}

				// check the initializer from object creation:
				//if (memberAccessSyntax?.Type != null)
				//{
				//	ti = analysisContext.SemanticModel.GetTypeInfo(variable.Initializer.Value);
				//	if (TryReportPrereleaseAttributeDiagnostics(analysisContext, fieldDeclaration, identifier, ti, typeRule, assemblyRule, "Field"))
				//		return;
				//}
			}
		}

		private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext analysisContext)
		{
			var typeRule = UseOfPrereleaseReturnTypeRule;
			var assemblyRule = UseOfPrereleaseAssemblyReturnTypeRule;

			// check method return type
			var methodDeclaration = new DeclarationSyntaxWrapper(analysisContext.SemanticModel, (MethodDeclarationSyntax) analysisContext.Node);
			if (TryReportPrereleaseAttributeDiagnostics(analysisContext, methodDeclaration, methodDeclaration.Identifier,
				methodDeclaration.Type, typeRule, assemblyRule))
			{
				return;
			}

			//if (TryReportPrereleaseAttributeDiagnostic(analysisContext, methodReturnTypeInfo, methodIdentifier, typeRule, assemblyRule))
			//	return;

			typeRule = UseOfPrereleaseTypeRule;
			assemblyRule = UseOfPrereleaseAssemblyRule;

			// check method-local declarations
			foreach (var node in methodDeclaration.DescendantNodes())
			{
				var localDeclarationStatementSyntax = node as LocalDeclarationStatementSyntax;
				if (localDeclarationStatementSyntax?.Declaration != null)
				{
					var variableDeclarationSyntax = localDeclarationStatementSyntax.Declaration;
					if (variableDeclarationSyntax.Variables.Count != 1)
					{
						continue;
					}
					var localDeclaration = new DeclarationSyntaxWrapper(analysisContext.SemanticModel, localDeclarationStatementSyntax);

					if (TryReportPrereleaseAttributeDiagnostics(analysisContext, localDeclaration,
						localDeclaration.Identifier, localDeclaration.Type, typeRule, assemblyRule))
					{
						return;
					}
				}
				// TODO: maybe check the initializer?
			}

			typeRule = UseOfPrereleaseTypeRule;
			assemblyRule = UseOfPrereleaseAssemblyRule;
			// rule for parameter?

			// check parameters
			var parameters = ((MethodDeclarationSyntax)methodDeclaration).ParameterList; // TODO: support parameters
			foreach (var parameter in parameters.Parameters)
			{
				var identifier = parameter.Identifier;
				var ti = analysisContext.SemanticModel.GetTypeInfo(parameter.Type);
				if (TryReportPrereleaseAttributeDiagnostics(analysisContext, methodDeclaration, identifier, ti.Type, typeRule, assemblyRule, "Parameter"))
				{
					return;
				}
			}
		}

		/// usage
		private void TryReportPrereleaseAttributeDiagnostics(SyntaxNodeAnalysisContext analysisContext, ISymbol symbol, SyntaxToken identifier, string identifierContext)
		{
			var attributes = symbol.GetAttributes();
			string attributeName;
			if (TryGetPrereleaseAttributeName(attributes, out attributeName))
			{
				var diagnostic = Diagnostic.Create(UseOfPrereleaseMemberRule,
					identifier.GetLocation(),
					identifier.ValueText, symbol.ToString(), attributeName, identifierContext);

				analysisContext.ReportDiagnostic(diagnostic);
				return;
			}
			// TODO: check if used item is in prerelease context instead.
			return;
		}

		/// declaration
		private bool TryReportPrereleaseAttributeDiagnostics(SyntaxNodeAnalysisContext analysisContext,
			DeclarationSyntaxWrapper node, SyntaxToken identifier, 
			ITypeSymbol type, DiagnosticDescriptor typeRule,
			DiagnosticDescriptor assemblyRule, string identifierContext = "Variable")
		{
			if (IsNodeInPrereleaseContext(analysisContext, node))
			{
				return false;
			}
			if (type == null)
			{
				return false;
			}
			var attributes = type.GetAttributes();
			if (attributes == null)
			{
				return false;
			}

			string attributeName;
			if (TryGetPrereleaseAttributeName(attributes, out attributeName))
			{
				var diagnostic = Diagnostic.Create(typeRule,
					identifier.GetLocation(),
					identifier.ValueText, type.ToString(), attributeName, identifierContext);

				analysisContext.ReportDiagnostic(diagnostic);
				return true;
			}

			// check members
			var attributeNames = node.AttributeLists.AttributeFullNames(analysisContext);
			if (attributeNames.Any())
			{
				if (TryGetPrereleaseAttributeName(attributeNames, out attributeName))
				{
					var diagnostic = Diagnostic.Create(assemblyRule,
						identifier.GetLocation(),
						identifier.ValueText, type.ToString(), attributeName, identifierContext);

					analysisContext.ReportDiagnostic(diagnostic);
					return true;
				}
			}
			attributes = type.ContainingAssembly.GetAttributes();
			if (TryGetPrereleaseAttributeName(attributes, out attributeName))
			{
				var diagnostic = Diagnostic.Create(assemblyRule,
					identifier.GetLocation(),
					identifier.ValueText, type.ToString(), attributeName, identifierContext);

				analysisContext.ReportDiagnostic(diagnostic);
				return true;
			}
			return false;
			//var attributeNames = node.AttributeLists.SelectMany(e => e.Attributes)
			//		.Select(e => analysisContext.SemanticModel.GetTypeInfo(e).Type.ToString());
			//string attributeName;
			//if (TryGetPrereleaseAttributeName(attributeNames, out attributeName))
			//{
			//	var diagnostic = Diagnostic.Create(typeRule,
			//		node.GetLocation(),//identifier.GetLocation(),
			//		node.GetText().ToString(), node.ToFullString(),//identifier.ValueText, type.ToString(),
			//		attributeName, identifierContext);

			//	analysisContext.ReportDiagnostic(diagnostic);
			//	return true;
			//}
			////var @namespace = node.Parent as NamespaceDeclarationSyntax;
			////if (TryGetPrereleaseAttributeName(attributes, out attributeName))
			////{
			////	var diagnostic = Diagnostic.Create(assemblyRule,
			////		identifier.GetLocation(),
			////		identifier.ValueText, type.ToString(), attributeName, identifierContext);

			////	analysisContext.ReportDiagnostic(diagnostic);
			////	return true;
			////}
			//return false;
		}

		private bool IsNodeInPrereleaseContext(SyntaxNodeAnalysisContext analysisContext, DeclarationSyntaxWrapper node)
		{
			// first check this node
			bool parentsChecked = false;

			while (true)
			{
				var attributeNames = node.AttributeLists.AttributeFullNames(analysisContext);
				if ((CSharpSyntaxNode)node is BaseTypeDeclarationSyntax && !attributeNames.Any())
				{
					var compilationUnit = node.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();
					if (compilationUnit != null) // ever?
					{
						attributeNames = compilationUnit.AttributeLists.AttributeFullNames(analysisContext);
					}
				}
				string attributeName;
				if (attributeNames.Any() &&
				    TryGetPrereleaseAttributeName(attributeNames, out attributeName))
				{
					return true;
				}

				if (parentsChecked)
				{
					return false;
				}

				// then check parent node
				node = node.Parent;
				parentsChecked = true;
			}
		}

		//private bool TryReportPrereleaseAttributeDiagnostic(SyntaxNodeAnalysisContext analysisContext, TypeInfo ti, SyntaxToken identifier, DiagnosticDescriptor typeRule, DiagnosticDescriptor assemblyRule, string identifierContext = "Variable")
		//{
		//	if (!IsNodeInPrereleaseContext(analysisContext, node))
		//		return false;
		//	if (ti.Type == null)
		//		return false;
		//	var attributes = ti.Type.GetAttributes();
		//	if (attributes == null)
		//		return false;
		//	string attributeName;
		//	var parent = identifier.Parent as MemberDeclarationSyntax;
		//	if (parent != null)
		//	{
		//		var t = analysisContext.SemanticModel.GetTypeInfo(parent);
		//		ClassDeclarationSyntax @class = parent.Parent as ClassDeclarationSyntax;
		//		t = analysisContext.SemanticModel.GetTypeInfo(@class);
		//	}
		//	if (TryGetPrereleaseAttributeName(attributes, out attributeName))
		//	{
		//		var diagnostic = Diagnostic.Create(typeRule,
		//			identifier.GetLocation(),
		//			identifier.ValueText, ti.Type.ToString(), attributeName, identifierContext);

		//		analysisContext.ReportDiagnostic(diagnostic);
		//		return true;
		//	}
		//	attributes = ti.Type.ContainingAssembly.GetAttributes();
		//	if (TryGetPrereleaseAttributeName(attributes, out attributeName))
		//	{
		//		var diagnostic = Diagnostic.Create(assemblyRule,
		//			identifier.GetLocation(),
		//			identifier.ValueText, ti.Type.ToString(), attributeName, identifierContext);

		//		analysisContext.ReportDiagnostic(diagnostic);
		//		return true;
		//	}
		//	return false;
		//}

		private bool TryGetPrereleaseAttributeName(IEnumerable<string> attributeNames, out string attributeName)
		{
			attributeName =
				attributeNames.Where(e => _prereleaseTypeNames.Contains(e.ToString()))
					.Select(e => e.ToString())
					.FirstOrDefault();
			return attributeName != null;
		}

		private bool TryGetPrereleaseAttributeName(ImmutableArray<AttributeData> attributes, out string attributeName)
		{
			if (attributes.Any(e => _prereleaseTypeNames.Contains(e.AttributeClass.ToString())))
			{
				attributeName =
					attributes.Where(e => _prereleaseTypeNames.Contains(e.AttributeClass.ToString()))
						.Select(e => e.AttributeClass.ToString())
						.First();
				return true;
			}
			attributeName = null;
			return false;
		}
	}
}
