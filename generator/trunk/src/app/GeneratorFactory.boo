namespace Generator

import System
import System.IO
import System.Text
import System.Reflection
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Generator.Extentions

class GeneratorFactory:
	
	static def CreateAndRun(argv as (string)) as int:
		if argv.Length == 0:
			ListGenerators()
			return -1
		
		name = argv[0].ToClassName()
		args = argv[1:]
		script = GetGeneratorScriptFile(name)
		
		if not File.Exists(script):
			print "Generator ${name} not found"
			return -1
		
		generator = Compile(script)
		
		if argv.Length == 1:
			generator.PrintUsage()
			return -1
		try:
			generator.Init(args)
			generator.Run()
		except ex as Exception:
			print 'An error occured while running the generator:'
			print ex.Message
			return -1
		return 0
	
	private static def ListGenerators():
		print 'Usage: generate GeneratorName [Arguments...]'
		print
		print 'Available generators:'
		for d in Directory.GetDirectories(ScriptBasePath):
			name = DirectoryInfo(d).Name
			try:
				gen = Compile(GetGeneratorScriptFile(name))
				print gen.GeneratorName.PadLeft(10), ':',  gen.Help()
			except Exception:
				pass
	
	public static ScriptBasePath as string:
		get:
			asmpath = Path.GetDirectoryName(typeof(GeneratorBase).Assembly.Location)
			return Path.Combine(asmpath, "../Generators/".ToPath())
		
	private static def GetGeneratorScriptFile(name as string):
		return Path.Combine(ScriptBasePath, "${name}/${name}Generator.boo")
			
	private static def Compile(script as string) as GeneratorBase:
		code = StringBuilder()
		
		# Adds default imports
		code.Append('import Generator;')
		code.Append('import Generator.Extentions;')
		code.Append('import Config;')
		
		using reader = StreamReader(script):
			code.Append(reader.ReadToEnd())
		
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(FileInput("${ScriptBasePath}/Config.boo"))
		compiler.Parameters.Input.Add(StringInput(script, code.ToString()))
		compiler.Parameters.References.Add(typeof(GeneratorBase).Assembly)
		compiler.Parameters.Pipeline = CompileToMemory()
		
		ctx = compiler.Run()
		
		if len(ctx.Errors) > 1:
			print "Compilation errors!"
			for e in ctx.Errors:
				print e unless e.Code == "BCE0028" #No entry point
		
		if ctx.GeneratedAssembly is null:
			raise GeneratorException("Can't compile generator")
		
		genType = ctx.GeneratedAssembly.GetTypes()[1]
		generator as GeneratorBase = genType()
		
		return generator

