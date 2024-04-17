#!/usr/bin/env pwsh
# Copyright (c) 2024 Roger Brown.
# Licensed under the MIT License.

trap
{
	throw $PSItem
}

function Assert
{
	Param ([parameter(Mandatory=$true)][ScriptBlock]$assertion)

	if ( -not ( & $assertion ) )
	{
		throw $assertion
	}
}

[byte[][]]$value = @( @(1,2) , @(3,4,5) )

$value | Format-Hex

Assert { 2 -eq $value.Count }
Assert { 2 -eq $value[0].Count }
Assert { 3 -eq $value[1].Count }

$value | Join-Array -Type ([byte]) -OutVariable out | Format-Hex

Assert { 1 -eq $out.Count }
Assert { 5 -eq $out[0].Count }
Assert { $out[0] -is [byte[]] }

$null | Join-Array -Type ([byte]) -OutVariable out

Assert { $null -ne $out }
Assert { 1 -eq $out.Count }
Assert { $null -ne $out[0] }
Assert { 0 -eq $out[0].Count }
Assert { $out[0] -is [byte[]] }

$value | Join-Array -Type ([byte]) -OutVariable out | Format-Hex

Assert { 1 -eq $out.Count }
Assert { 5 -eq $out[0].Count }
Assert { $out[0] -is [byte[]] }

Write-Output 'Hello World'.Split(' ') -NoEnumerate | Join-Array -Type ([string]) -OutVariable out

Assert { 1 -eq $out.Count }
Assert { 2 -eq $out[0].Count }
Assert { $out[0] -is [string[]] }

@( @(,'Goodbye'),@('Cruel','World') ) | Join-Array -Type ([string]) -OutVariable out

Assert { 1 -eq $out.Count }
Assert { 3 -eq $out[0].Count }
Assert { $out[0] -is [string[]] }

@( (,'ABC'.ToCharArray()) )  | Join-Array -Type ([char]) -OutVariable out

Assert { 1 -eq $out.Count }
Assert { 3 -eq $out[0].Count }
Assert { $out[0] -is [char[]] }

$hello = New-Object System.String -ArgumentList $out

Assert { $hello -eq 'ABC' }
