# Lo nuevo en C# 8.0 - Pattern Matching

Pattern Matching es un concepto que introdujo C# 7 y en esta nueva versión recibe una actualización con nuevas características.

El concepto de Pattern Matching puede separarse en subtipos:
* __Type Matching__: permite "matchear" el tipo de la clase de un objecto
* __Property Matching__: permite inspeccionar las propiedades del objeto durante la comprobación
* __Tuple Matching__: permite matching usando tuplas
* __Positional Matching__: Deconstruir un objeto y verificar sus propiedades

Primero de todo, creemos el entorno de los ejemplos, para ello creamos 3 clases que representan figuras ge

```csharp
class Cuadrado
{
    public Cuadrado(double lado) => Lado = lado;

    public double Lado { get; }
}

class Circulo
{
    public Circulo(double radio) => Radio = radio;
    
    public double Radio { get; }
}

class Rectangulo
{
    public Rectangulo (double baseRect, double altura)
    {
        Base = baseRect;
        Altura = altura;
    }

    public double Base { get; }

    public double Altura { get; }
}
```

## Ejemplos de Pattern Matching


### 1. Matching en sentencias "if" usando "is"

```csharp
if (figura is Cuadrado c)
    return c.Lado * c.Lado;
else if (figura is Circulo circ)
    return circ.Radio * circ.Radio * Math.PI;
else if (figura is Rectangulo rect)
    return rect.Altura * rect.Base;

return null;
```

### 2. Matching en sentencias "switch"

```csharp
switch (figura)
{
    case Cuadrado cuad:
        return cuad.Lado * cuad.Lado;
    case Circulo circ:
        return circ.Radio * circ.Radio * Math.PI;
    case Rectangulo rect:
        return rect.Altura * rect.Base;
    default:
        return null;
}
```

### 3. Matching usando "when" en "switch/case"

```csharp
switch (figura)
{
    case Cuadrado cuad when cuad.Lado == 0:
    case Circulo circ when circ.Radio == 0:
    case Rectangulo rect when rect.Base == 0 || rect.Altura == 0:
        return 0;
    case Cuadrado cuad:
        return cuad.Lado * cuad.Lado;
    case { }:
    case null:
    default:
        return null;
}
```

> __Nota:__ podemos ver el uso de `case {}` para "matchear" con cualquier objeto y el uso de `null` para "matechear" cuando la propiedad _figura_ es nulo.

### 4. Matching usando "var"en sentencias "swith/case"

```csharp
var texto = "texto";
switch (texto)
{
    case var t when string.IsNullOrEmpty(t):
        return 0;
    case var s when s.Trim().ToLower() == "texto":
        return 1;
    default:
        return 2;
}
```

### 5. Property Matching

```csharp
public double example(object figura) => figura switch
{
    Cuadrado { Lado: 0 } => 0,
    Cuadrado { Lado: 1 } => 1,
    Rectangulo { Base: 0 } => 0,
    Rectangulo rect when ((rect.Altura / rect.Base) < 1) => 10,
    _ => 0,
};
```

En este ejemplo mezclamos varias posibilidades (Type Matching y Property Matching) y vemos una nueva forma de escribir el switch. 😀

> __Nota__: el nuevo matching "_" es un sinónimo del conocido "default".

Veamos un ejemplo en donde únicamente se hace Property Matching

```csharp
public double? example(Cuadrado cuadrado) => cuadrado switch
{
    { Lado: 0 } => 0,
    { Lado: 1 } => 1,
    null => null,
    _ => cuadrado.Lado * cuadrado.Lado
};
```

### 6. Tuple Matching
```csharp
static State ChangeState(State current, Transition transition, bool hasKey) =>
    (current, transition, hasKey) switch
    {
        (Opened, Close,  _)    => Closed,
        (Closed, Open,   _)    => Opened,
        (Closed, Lock,   true) => Locked,
        (Locked, Unlock, true) => Closed,
        _ => throw new InvalidOperationException($"Invalid transition")
    };
```

### 7. Positional Matching
Este patrón utliza `deconstruct` [pattern](https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct) para hacer la separacion de los datos.

```cssharp
class Point
{
    public int X { get; }
    public int Y { get; }
    public Point(int x, int y) => (X, Y) = (x, y);
    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
}

static string Display(Point o) => o switch
{
    Point(0, 0)         => "origin",
    Point(var x, var y) => $"({x}, {y})",
    _                   => "unknown"
};
```

