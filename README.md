# rhubarb-geek-nz/Joinery
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
Join-Dictionary [-InputObject <psobject>] [<CommonParameters>]

Join-Dictionary -Dictionary <IDictionary> [-PassThru] [-InputObject <psobject>] [<CommonParameters>]
```

The output is a dictionary with the values added.

## ConvertTo-List

Collects the input pipeline and creates a single list.

```
ConvertTo-List [-BaseObject] [-Type <type>] [-InputObject <psobject>] [<CommonParameters>]

ConvertTo-List -List <IList> [-PassThru] [-BaseObject] [-InputObject <psobject>] [<CommonParameters>]
```

## Join-Boolean

Join-Boolean combines boolean values using boolean logic. Not will turn an And into a Nand, an Or into a Nor. With PassThru, Not creates an invertor.

```
Join-Boolean [-InputList] <bool[]> -PassThru [-Not] [-NoEnumerate] [-DefaultValue <bool>] [<CommonParameters>]

Join-Boolean -InputValue <bool> -PassThru [-Not] [-DefaultValue <bool>] [<CommonParameters>]

Join-Boolean -InputValue <bool> -And [-Not] [-DefaultValue <bool>] [<CommonParameters>]

Join-Boolean -InputValue <bool> -Or [-Not] [-DefaultValue <bool>] [<CommonParameters>]

Join-Boolean -InputValue <bool> -Xor [-Not] [-DefaultValue <bool>] [<CommonParameters>]

Join-Boolean [-InputList] <bool[]> -And [-Not] [-DefaultValue <bool>] [<CommonParameters>]

Join-Boolean [-InputList] <bool[]> -Or [-Not] [-DefaultValue <bool>] [<CommonParameters>]

Join-Boolean [-InputList] <bool[]> -Xor [-Not] [-DefaultValue <bool>] [<CommonParameters>]

Join-Boolean -And [-Not] [-DefaultValue <bool>] [<CommonParameters>]

Join-Boolean -Or [-Not] [-DefaultValue <bool>] [<CommonParameters>]

Join-Boolean -Xor [-Not] [-DefaultValue <bool>] [<CommonParameters>]
```

See [test.ps1](test.ps1) for examples.
