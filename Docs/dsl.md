# AkiBT DSL

AkBT.DSL is a domain-specific language designed for AkiBT, use standard AST (Abstract Syntax Tree) to build behavior tree directly instead of generating json as intermediate.

Inspired by Game AI Pro and .Net's C# version for LLVM's official tutorial of Kaleidoscope.

## What is DSL

A Domain-Specific Language (DSL) is a computer language that's targeted to a particular kind of problem, rather than a general purpose language that's aimed at any kind of software problem.

[See Reference Article](https://martinfowler.com/dsl.html)

## How to use

```
///
It's a comment
The following is a behavior tree written using AkiBTDSL
///
Vector3 destination (0,0,0)
Vector3 myPos (0,0,0)
Float distance 1
Vector3 subtract (0,0,0)
Parallel(children:[
	Sequence(children:[
		Vector3Random(xRange:(-10,10),yRange:(0,0),zRange:(-10,10),operation:1,
		storeResult=>destination ),
		DebugLog(logText:"This is a log: agent get a new destination."),
		TimeWait(waitTime:10)
	]),
	Sequence(children:[
		Sequence(children:[
			TransformGetPosition(storeResult=>myPos),
			 Vector3Operator(operation:1,firstVector3=>myPos,
				secondVector3=>destination,storeResult=>subtract),
			Vector3GetSqrMagnitude(vector3=>subtract,result=>distance)
		]),
		Selector(abortOnConditionChanged: false, children:[
			FloatComparison(evaluateOnRunning:false,float1=>distance,
				float2:4,operation:5,child:
				Sequence(abortOnConditionChanged:false,children:[
					NavmeshStopAgent(isStopped:false),
					NavmeshSetDestination(destination=>destination)
				])
			),
			NavmeshStopAgent(isStopped:true)
		])
	])
])


```

The above behavior tree is the patrol AI behavior tree in AkiBT Example, it will get a new position every 10 seconds and move to it, if the distance from the target point is less than 2, it will stop

The main body of DSL can be divided into two parts, public variables and nodes. The declaration of public variables needs to specify the type, name and value.

If the type is wrapped with `$`, global variables will be bound at runtime, for example:

```
$Vector3$ TargetPosition (0,0,0)
```

If the variable type is Object (SharedObject), you can declare the type name before declaring the value. For example:
```
Object navAgent "UnityEngine.AIModule,UnityEngine.AI.NavMeshAgent" Null
```

The type name's format is `{Assembly Name},{NameSpace}.{ClassName}` or use `Type.AssemblyQualifiedName`.

Then the variable is set to global and has the ``NavMeshAgent`` type restriction.

For nodes, we will skip the Root node (because all behavior trees enter from the Root), and start writing directly from the Root's child nodes.


For a node, you need to declare its type.


For ordinary variables that do not use the default value of the node, you need to declare its name (or use AkiLabelAttribute to alter field's name) and add ':' to assign


For the shared variable in the node, if you don’t need to refer to the shared variable of the public variable, you can assign it directly, for example

```
TimeWait(waitTime:10)
```

For shared variables that need to be referenced, use the '=>' symbol plus the name of the public variable that needs to be referenced, for example
```
NavmeshSetDestination(destination=>myDestination)
```

## Compatible with AkiLabel

DSL supports use `AkiLabelAttribute`'s value as name to build behavior tree. For example:

```
Vector3 玩家位置 (0,0,0)
序列 (子节点:[
    获取玩家位置 (位置=>玩家位置),
    移动至玩家(目标=>玩家位置)
])
```

Which is quite cool!

## Reference

https://llvm.org/docs/tutorial/MyFirstLanguageFrontend/index.html

https://github.com/dotnet/LLVMSharp/blob/main/samples/KaleidoscopeTutorial