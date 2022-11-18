{ pkgs }: {
	deps = [
		pkgs.run
  pkgs.dotnet-sdk
    pkgs.omnisharp-roslyn
	];
}