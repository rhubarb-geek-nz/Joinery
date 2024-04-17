# rhubarb-geek-nz.Joinery
Join-Array tool for PowerShell

This tool joins arrays in the same manner as `Join-String` by simple concatenation.

```
Join-Array -InputObject <IEnumerable> -Type <type> [<CommonParameters>]
```

The output is single array of type `<type>`. The input pipeline records must be able to be casted to an `IEnumerable<type>`.

```
PS> ( $null | Join-Array -Type ([byte]) ).GetType().FullName
System.Byte[]
```
