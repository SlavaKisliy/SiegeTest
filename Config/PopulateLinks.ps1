$base = "https://host.docker.internal:5001/api/Customers/";
$baseNewCache = "https://host.docker.internal:5001/api/NewCacheCustomers/";
$fileContent = "";
$fileContentNewCache = "";

For ($i=0; $i -le 20; $i++) {
	$line = $base + ($i + 1);
	$fileContent += $line + "`n";
	
	$line = $baseNewCache + ($i + 100);
	$fileContentNewCache += $line + "`n";
}

Set-Content links.txt $fileContent
Set-Content linksNewCache.txt $fileContentNewCache