require 'albacore'

msbuild :build do |msb|
  msb.properties :configuration => :Release
  msb.targets :Clean, :Build
  msb.solution = 'Trappings.sln'
end

directory 'build/lib/net40'

task :package => [:build, 'build/lib/net40'] do
  cp FileList['Trappings/bin/Release/Trappings.*'], 'build/lib/net40'
  cp 'Trappings.nuspec', 'build'
  cd 'build'
  `../.nuget/NuGet.exe pack`
end
