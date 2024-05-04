# rhubarb-geek-nz.Joinery
Join Tools for PowerShell

## Join-Array

Join-Array joins arrays in the same manner as `Join-String` by simple concatenation.

```
Join-Array -InputObject <IEnumerable> -Type <type> [<CommonParameters>]
```

The output is single array of type `<type>`. The input pipeline records must be able to be casted to an `IEnumerable<type>`.

```
PS> ( $null | Join-Array -Type ([byte]) ).GetType().FullName
System.Byte[]
```

## Join-Dictionary

Join-Dictionary adds dictionary entries and properties into a target dictionary.

```
Join-Dictionary [-Dictionary <IDictionary>] [-PassThru] [-InputObject <psobject>] [<CommonParameters>]
```

The output is a dictionary with the values added.
