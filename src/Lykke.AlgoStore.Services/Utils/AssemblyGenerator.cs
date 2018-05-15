using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Lykke.AlgoStore.Services.Utils
{
    public class AssemblyGenerator
    {
        private readonly List<PortableExecutableReference> _references;

        public AssemblyGenerator()
        {
            _references = new List<PortableExecutableReference>();
        }

        public void ReferenceAssembly(Assembly assembly)
        {
            _references.Add(MetadataReference.CreateFromFile(assembly.Location));
        }

        public void ReferenceAssemblyContainingType<T>()
        {
            ReferenceAssembly(typeof(T).Assembly);
        }

        public Assembly Generate(string code)
        {
            var assemblyName = Path.GetRandomFileName();
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var references = _references.ToArray();
            var compilation = CSharpCompilation.Create(assemblyName, new[] {syntaxTree}, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


            using (var stream = new MemoryStream())
            {
                var result = compilation.Emit(stream);

                if (!result.Success)
                {
                    var failures = result.Diagnostics
                        .Where(x => x.IsWarningAsError || x.Severity == DiagnosticSeverity.Error);

                    var message = string.Join(Environment.NewLine, failures.Select(x => $"{x.Id}: {x.GetMessage()}"));

                    throw new InvalidOperationException($"Compilation failures!{Environment.NewLine}{message}{Environment.NewLine}Code:{Environment.NewLine}{code}");
                }

                stream.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(stream.ToArray());
            }
        }
    }
}
