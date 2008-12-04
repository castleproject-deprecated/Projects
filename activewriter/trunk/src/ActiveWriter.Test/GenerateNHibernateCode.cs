using System;
using System.Collections.Generic;
using System.IO;
using Altinoren.ActiveWriter;
using Altinoren.ActiveWriter.CodeGeneration;
using Microsoft.CSharp;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using NUnit.Framework;

namespace ActiveWriter.Test
{
    [TestFixture]
    public class GenerateNHibernateCode
    {
        private string _tempDirectory;
        private string _intermediateDirectory;
        private System.CodeDom.Compiler.CodeDomProvider _codeDomProvider;
        private Dictionary<string, string> _references = new Dictionary<string, string>();

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _intermediateDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            if (Directory.Exists(_tempDirectory))
                Directory.Delete(_tempDirectory);

            if (Directory.Exists(_intermediateDirectory))
                Directory.Delete(_intermediateDirectory);

            Directory.CreateDirectory(_tempDirectory);
            Directory.CreateDirectory(_intermediateDirectory);

            _codeDomProvider = new CSharpCodeProvider();

            AddReferences("Castle.ActiveRecord");
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Directory.Delete(_tempDirectory, true);
            Directory.Delete(_intermediateDirectory, true);
        }

        private Transaction _transaction;
        private Model _model;
        private Store _store;
        private List<string> _codeFiles;
        private List<string> _resources;

        [SetUp]
        public void SetUp()
        {
            _store = new Store(typeof(CoreDesignSurfaceDomainModel),
                                    typeof(ActiveWriterDomainModel));

            _transaction = _store.TransactionManager.BeginTransaction("Test Model");
            _model = new Model(_store);
            _model.Target = CodeGenerationTarget.NHibernate;
            _codeFiles = new List<string>();
            _resources = new List<string>();
        }

        [TearDown]
        public void TearDown()
        {
            _transaction.Dispose();
        }

        [Test]
        public void ShouldGenerateClassWithProperties()
        {
            CreateClass("FooClass", "FooId");

            // Generate code
            string generatedClasses = GenerateCode();

            AssertFragments(
                generatedClasses,
                "namespace ModelNamespace",

                    "class FooClass",
                        "string FooId");

            AssertResources("FooClass.hbm.xml");
        }

        [Test]
        public void ShoulGenerateTwoClasses()
        {
            ModelClass fooClass = CreateClass("FooClass", "FooId");
            ModelClass barClass = CreateClass("BarClass", "BarId");

            string generatedClasses = GenerateCode();

            AssertFragments(
                generatedClasses,
                "namespace ModelNamespace",

                    "class FooClass",
                        "string FooId",

                    "class BarClass",
                        "string BarId");

            AssertResources("FooClass.hbm.xml", "BarClass.hbm.xml");
        }

        [Ignore("Test not implemented yet")]
        [Test]
        public void ShoulGenerateTwoClassesWithOneToOneRelation()
        {

        }

        [Test]
        public void ShouldGenerateTwoClassesWithOneToManyRelation()
        {
            ModelClass fooClass = CreateClass("FooClass", "FooId");
            ModelClass barClass = CreateClass("BarClass", "BarId");
            CreateOneToManyRelation(fooClass, barClass);

            string generatedClasses = GenerateCode();

            AssertFragments(
                generatedClasses,
                "namespace ModelNamespace",

                    "class FooClass",
                        "string FooId",

                    "class BarClass",
                        "string BarId");

            AssertResources("FooClass.hbm.xml", "BarClass.hbm.xml");
        }

        [Ignore("Test not implemented yet")]
        [Test]
        public void ShouldGenerateTwoClassesWithManyToOneRelation()
        {
            
        }

        [Ignore("Test not implemented yet")]
        [Test]
        public void ShouldGenerateTwoClassesWithManyToManyRelation()
        {
            
        }

        protected void AssertFragments(string actualCode, params string[] fragments)
        {
            Array.ForEach(fragments, fragment =>
                StringAssert.Contains(fragment, actualCode));
        }

        protected void AssertCodeFiles(params string[] codeFiles)
        {
            CollectionAssert.AreEquivalent(codeFiles, _codeFiles, "Generated code files do not match expectation");
        }

        protected void AssertResources(params string[] resources)
        {
            CollectionAssert.AreEquivalent(resources, _resources, "Generated resource files do not match expectation");
        }

        private void AddReferences(params string[] assemblyNames)
        {
            Array.ForEach(assemblyNames, assemblyName =>
            {
                string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName + ".dll");
                _references[assemblyPath] = assemblyName;
            });
        }

        private ModelClass CreateClass(string className, string primaryKeyName, params string[] properties)
        {
            // Class FooClass
            ModelClass modelClass = new ModelClass(
                _store,
                new PropertyAssignment(ModelClass.NameDomainPropertyId, className)
                );

            // ID Property
            modelClass.Properties.Add(new ModelProperty(_store,
                new PropertyAssignment(ModelProperty.NameDomainPropertyId, primaryKeyName),
                new PropertyAssignment(ModelProperty.UniqueKeyDomainPropertyId, primaryKeyName),
                new PropertyAssignment(ModelProperty.KeyTypeDomainPropertyId, KeyType.PrimaryKey)
            ));

            Array.ForEach(properties, property =>
                modelClass.Properties.Add(new ModelProperty(_store,
                    new PropertyAssignment(ModelProperty.NameDomainPropertyId, property)
            )));

            _model.Classes.Add(modelClass);

            return modelClass;
        }

        private void CreateOneToManyRelation(ModelClass fooClass, ModelClass barClass)
        {
            ManyToOneRelation relation = new ManyToOneRelation(fooClass, barClass);

            //new ElementLink(_model.Partition,
            //                new RoleAssignment[]
            //                    {
            //                        new RoleAssignment(Altinoren.ActiveWriter.ManyToOneRelation), 
            //                    },
            //                new PropertyAssignment[]
            //                    {
            //                    });
        }

        protected Model CreateModel(Action<Model> editModel)
        {
            Store store = new Store(typeof(CoreDesignSurfaceDomainModel),
                                    typeof(ActiveWriterDomainModel));

            using (store.TransactionManager.BeginTransaction("Test Model"))
            {
                Model model = new Model(store);

                editModel(model);

                return model;
            }
        }

        protected string GenerateCode()
        {
            CodeGenerationContext context = new CodeGenerationContext(
                _model,
                "ModelNamespace",
                "AssemblyNamespace",
                "AssemblyName",
                _tempDirectory,
                _intermediateDirectory,
                _codeDomProvider,
                true,
                _references);

            CodeGenerationHelper codeGenerationHelper = new CodeGenerationHelper(context);

            codeGenerationHelper.CompilerErrors += (source, errors) =>
                Assert.Fail("Error(s) while compiling: {0}", string.Join("\n", errors.ErrorStrings.ToArray()));

            codeGenerationHelper.CodeFileCreated += (source, args) => _codeFiles.Add(args.FileName);
            codeGenerationHelper.ResourceCreated += (source, args) => _resources.Add(args.FileName);

            return codeGenerationHelper.Generate();
        }

    }
}
