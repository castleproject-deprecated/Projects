namespace Generator

import System
import System.IO
import System.Reflection
import Generator.Extentions

abstract class GeneratorBase:
	[Property(Argv)]			_argv as (string)
	[Property(ScriptPath)]		_scriptPath as string
	[Property(TemplatesPath)]	_templatesPath as string
	[Property(Force)]			_force as bool
	
	_overwiteAll as bool = false
	
	def constructor():
		_scriptPath = Path.Combine(ScriptBasePath, GeneratorName)
		_templatesPath = "${ScriptPath}/Templates"
	
	# This method is called when the generator is created
	# it can be used to initialize arguments
	virtual def Init(argv as (string)):
		_argv = argv
	
	# Main method to run the generator
	abstract def Run():
		pass
	
	virtual def Usage() as string:
		return '[arguments]'
		
	virtual def Help() as string:
		return 'No help yet, maybe tomorrow!'
	
	#region Template properties
	GeneratorName as string:
		get:
			n = GetType().Name
			return n.Substring(0, n.Length - 'generator'.Length)
	
	ScriptBasePath as string:
		get:
			return GeneratorFactory.ScriptBasePath
	#endregion
	
	#region Template processing methods
	def Process(template as string, tofile as string):
		Process(template, tofile, false)
		
	def Process(template as string, tofile as string, keep as bool):
		cleanToFile = tofile.ToPath()
		t = Template(Path.Combine(_templatesPath, template.ToPath()))
		result = t.Process(CollectVariables())
		file = FileInfo(cleanToFile)
		
		if file.Exists:
			if keep:
				Log('exists', cleanToFile)
				return
			
			if file.Length == result.Length:
				if Force:
					Log('replace', cleanToFile)
				else:
					Log('same', cleanToFile)
					return
			else:
				if LogAndAskForOverwrite(cleanToFile):
					Log('replace', cleanToFile)
				else:
					return
		else:
			Log('create', cleanToFile)
		
		
		using w = StreamWriter(cleanToFile):
			w.Write(result)
	
	def Copy(file as string, topath as string):
		cleanFile = Path.Combine(_templatesPath, file.ToPath())
		cleanToPath = topath.ToPath()
		cleanToFile = Path.Combine(cleanToPath, FileInfo(cleanFile).Name)
		
		if File.Exists(cleanToFile):
			if LogAndAskForOverwrite(cleanToFile):
				Log('replace', cleanToFile)
			else:
				return
		else:
			Log('create', cleanToFile)
		
		File.Copy(cleanFile, cleanToFile, true)
		
	def CopyDir(dir as string, topath as string):
		MkDir(topath)
		source = Path.Combine(_templatesPath, dir.ToPath())
		for file in Directory.GetFiles(source):
			name = FileInfo(file).Name
			Copy(file, topath) if not name.StartsWith('.')
		for dir in Directory.GetDirectories(source):
			name = DirectoryInfo(dir).Name
			CopyDir(dir, Path.Combine(topath, name)) if not name.StartsWith('.')
		
	def MkDir(path as string):
		cleanPath = path.ToPath()
		if Directory.Exists(cleanPath):
			Log('exists', cleanPath)
		else:
			Log('create', cleanPath)
			Directory.CreateDirectory(cleanPath)
	#endregion
	
	#region Helper methods
	virtual def PrintUsage():
		print "Usage: generate ${GeneratorName} ${Usage()}"
		print Help()

	protected def Log(action as string, path as string):
		print action.PadLeft(10), path
	
	private def CollectVariables() as Hash:
		props = {}
		for p as PropertyInfo in GetType().GetProperties():
			props.Add(p.Name, p.GetValue(self, null))
		return props
	
	private def LogAndAskForOverwrite(path as string) as bool:
		return true if _overwiteAll
		
		question = "${'exists'.PadLeft(10)} ${path} Overwrite? [Ynaq] "
		answer = prompt(question).ToLower()
		if answer == 'y' or answer == '':
			return true
		elif answer == 'a':
			_overwiteAll = true
			return true
		elif answer == 'q':
			Environment.Exit(0)
			return false
	#endregion
