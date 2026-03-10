    <PackageReference Include="StyleCop.Analyzers" Version="1.1.118">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>

dotnet tool install -g csharpier
dotnet csharpier --check .

csharpier check ./src/


----

dotnet tool install -g JetBrains.ReSharper.GlobalTools

jb inspectcode QiWa.sln --config-create=./build/jb.yaml --no-updates -j=4 --no-build -e=INFO --format=Html --output=./build/report.html

