# Tangent Language

Tangent Version: 1.2

### 一. 什么是Tangent
Tangent Language是樱游社专为编写剧本而开发的一种指令式的程序语言，它的语法与yaml类似，但结构更为简单易懂，采用缩进作为嵌套格式符，采用键值对的形式表达指令。Tangent不可以单独作为一段程序执行，它是一种顶层语言，必须由其他语言作为载体，为Tangent提供关键词映射表和脚本语句执行方式，例如使用Lua执行脚本行和C#提供核心服务。
 
### 二. 基本语法
####1. 文件类型
Tangent语言脚本文件的标准扩展名为.tt。您编写的Tangent Script的命名应遵循这个格式：script.tt。
####2. 语句
Tangent语句采用yaml的格式，即键值对形式：“键：值”。
键是本条语句的指令，值是本条指a令的具体内容。综上，Tangent的关键词（指令）映射表除去保留关键词（见6）外，均由宿主语言所决定。例如：
```
	move: 10,0,10
	action: fight
```
### #3. 类
Tangent规定，一个文件中只允许定义一个类，类名与文件名无关，但必须定义在tt脚本开头的位置。类名在跨文件访问函数式是必要的。Tangent语言的类名可以接受命名中除了“.”（圆点）、“:”（冒号）、“$”（美元符号）以外任意utf-8字符，但是仍然建议不要使用空格并且采用英文命名。例如：
```properties
#文件：example01.tt
Script_01:
	$log_string:
		log: Hello World
		
		
#文件：example02.tt
Script_02:
	$main:
		include: example01.tt
		call: Script_01.$log_string
```
#### 4. 函数
函数是Tangent语言最基本的组成元素，函数的声明方式为“美元符号+函数名”，例如$function，Tangent语言的函数名可以接受命名中除了“.”（圆点）、“:”（冒号）、“$”（美元符号）以外任意utf-8字符，但是仍然建议不要使用空格并且采用英文命名。$main函数是一个Tangent脚本的入口点，然而当脚本作为被include的对象，主函数不会执行。例如：
```properties·
Script_03:
	$main:
		log:Hello World
```
#### 5. 变量
Tangent语言中一切都可以是“变量”，但是Tangent不提供保存某种数据或数据结构的类型。如果你需要存储一些变量或调用一些原生方法，你可以用过eval语句执行宿主语言的接口。在樱游社的标准Tangent语言执行库中（OpenWorld框架），每一个Tangent脚本都会附属于一个LuaTable（虚拟机中的独立空间），您可以在Tangent中使用eval指令无缝地调用Lua接口。

#### 6. 保留关键字
include 
eval
if
switch
while
case
default
true
false
call

#### 7. 注释
注释单独成行，以#开头，例如：
```
	#一条指令
	run: 123
```
### 三. 基础教程
#### 1. 接入关键词（程序组） 
TangentEnv是Tangent语言运行时的环境单例，您可以在此定义需要的关键词映射表。TangetKeywords是关键词集合类，您需要自己实例化本类，定义列表并将其设置为环境下的映射表。例如:
```c#
	var keywords = new TangentKeywords(
		("log", obj =>
		{
		    Debug.log($"Log: {obj.Value}");
		    obj.Finish();
		}));
	
	TangentEnv.SetKeywords(keywords);
```
Tangent映射表由一对键值组成，上述代码定义了1个用户指令“log”，这个指令为键，即关键词名，值为相应的执行函数（TangentAction）。
```c#
    public delegate void TangentAction(TangentEventArg arg);
```
TangentEventArgs是委托传入的函数，他的类构成如下：
```c#
	public class TangentEventArg
	{
	    public string Key { get; }
	    public string Value { get; }
	    public Dictionary<string, string> ArgPairs { get; }
	    
	    public void Finish();
	}
```
##### 1）变量  
1. **Key**：字符串类型，Key是当前指令的名称，在例2-2中，“move”为指令名。  
2. **Value**：字符串类型，Value是Tangent语言中所设置的值，在例2-2中，“10,0,10”为值。  
3. **ArgPairs**：字符串字典表，若当前指令值含有子键值对，这些子键值对会被存储到此处。例如：
```
	dosomething: move
		direction: up
		speed: 3
		delay: 1
```
以上例子中，direction、speed和delay就会被存储在此。  
##### 2）函数
1. **Finish()**：委托必须在每次行为执行结束后执行次方法，来告诉Tangent本指令已经执行完毕，该执行下一句了。

####2. 接入委托（程序组） 
TangentEnv中包含以下委托函数，需要用户进行定义
```c#
    public static Func<string, string> OnLoadScript;
    public static Func<string, object> OnEvalCond;
    public static Action<string, TangentScript> OnEvalScript;
```
1. **OnLoadScript**是脚本加载和include时的路径寻找逻辑，传入路径，函数应当返回脚本的文本内容。  
2. **OnEvalCond**是cond指令执行条件判断语句（通常是Lua语句）的委托，传入条件语句，函数应当返回一个bool值代表判断结果。  
3. **OnEvalScript**是eval指令执行语句时（通常是Lua语句）调用的函数，传入语句内容和脚本实例，宿主语言执行语句程序。

<font color="#FF0000">**注意**</font>：虽然Tangent能运行原生的脚本语言（通常是Lua）的语句，但这是不推荐的，程序应当为编剧封装好必要的方法供调用，换而言之，eval和cond中的内容**应当是**一个函数调用而**不是**一句句的去实现功能。

#### 3. 装载脚本（程序组）
通过TangentEnv下的静态方法装载脚本并运行。
##### 1） TangentScript Load(string filename)
传入文件名，通过上述委托OnLoadScript来查找读取文件。
##### 2） TangentScript LoadFile(string filename)
传入文件名，直接打开文件读取。
##### 3） TangentScript LoadText(string text, string filename)
传入文件内容并解析，同时也要传入一个文件名，用于显示错误信息。不推荐这个方法。

#### 4. 开始编写Tangent脚本
这是一个简单的HelloWorld代码示例，程序组成员推荐自己实现这整个过程，编剧可不用关心C#相关内容  
##### 1. 接入运行库(程序组) 
新建一个C#控制台工程，从Nuget安装NLua，在C#代码中我们先实现并执行以下方法。  
在这段程序里定义了四个关键词，Log、Conv、Action、Avatar，分别用来模拟打印调试信息、对话文字、人物动作、切换头像的操作。这是之后执行脚本的基础。装载完成后调用Step执行，当Step()返回True时，程序运行结束。
```c#
private void Define()
{
    var keywords = new TangentKeywords(
        ("log", obj =>
        {
            Console.WriteLine($"Log: {obj.Value}");
            obj.Finish();
        }),
        ("conv", obj =>
        {
            Console.WriteLine($"Disp Conversation: {obj.Value}");
            obj.Finish();
        }),
        ("action", obj =>
        {
            Console.WriteLine($"Run Action: {obj.Value}");
            obj.Finish();
        }),
        ("avatar", obj =>
        {
            Console.WriteLine($"Change Avatar:{obj.Value}");
            obj.Finish();
        }));

    TangentEnv.SetKeywords(keywords);

    lua = new Lua();
    lua.RegisterFunction("LPrint", this, GetType().GetMethod("LuaPrint"));

	TangentEnv.OnLoadScript = s =>
	{
		if (!File.Exists(s))
			Console.WriteLine($"File \"{s}\" not found");
		
		return File.ReadAllText(s);
	};
	
    TangentEnv.OnEvalCond = s => (bool)lua.DoString($"return {s}")[0];
    TangentEnv.OnEvalScript = s => lua.DoString(s);

    var script = TangentEnv.LoadFile(YOUR_FILE_PATH); //替换为你的文件路径
    script.Step(); //开始执行
}

public void LuaPrint(string text)
{
    Box.AppendText($"Lua Print: {text}", Brushes.Brown);
}
```

##### 2. 编写Hello World  
新建一个脚本，自定义一个文件名，后缀.tt，输入以下脚本内容：  
```
myFirstTangent:
	#主函数，整个程序的入口点
	$main:
		log: Hello World!
	
```
通过上述程序执行，得到结果
```md
Hello World！
```

##### 3. 使用自定义指令  
```properties
test: 
  $main:
    action: 镜头移动到灵梦
    avatar: avatar_reimu_04
    conv: 呀，铃奈庵叕叕叕叕着火了！

    action: 镜头移动到本居小铃
    avatar: avatar_kosuzu_01
    conv: 得想个办法逃出去！
```
##### 4. 调用函数  
函数调用采用call指令，值为函数名
```properties
test: 
  $main:
    call: $func_reimu
    call: $func_kosuzu

  $func_reimu:
    log: 灵梦函数执行！
    log: Do Something...
    log: 灵梦函数结束！
    call: $func_kosuzu

  $func_kosuzu:
    log: 小铃函数执行！
    log: Do Something...
    log: 小铃函数结束！
```

##### 5. 选择结构（if）  
一个是非判断的语言结构，当cond指令为true时执行true块，反之执行false块
```properties
test:
    $main:
        if:
            cond: 1+1 > 2
            true: 
                call: $onTrue
            false:
                call: $onFalse
    $onTrue:
        log: 1+1 < 2
    $onFalse:
        log: 9!
```

##### 6. 选择结构（switch-case)  
这里的Switch与C Like语言的结构不同，这里更多的是提供一个多条件的匹配，也就是简化一个if嵌套的写法，而非只能针对一个算式的选择（像例子中这样是针对一个计算来选择）。当有case的cond为真时，case中的prog块执行，然后跳出switch。如果没有条件成立，执行default块。default块可以省略，如果没有default块且全部条件均不为真，则switch-case不执行，直接被跳过。
```properties
test:
    $main:
        switch:
            case:
                cond: 1+1 == 2
                prog: 
                    log: 999!
            case:
                cond: 1+1 == 9
                prog:
                    log: 99999!
            default:
                prog:
                    log: Correct!
```

##### 7. 循环结构（while）
循环结构必须包含一个条件语句（cond节点），每次循环开始时判断cond是否成立，如果cond成立则执行循环，反之结束循环，循环指令块为prog节点。
```properties
test: 
  $main:
  	eval: i = 0
	while:
		cond: i < 5
		prog:
			log: loop
			eval: LPrint(tostring(i))
			eval: i = i + 1
```

##### 8. 调用C#接口（通过Lua）
```properties
test: 
  $main:
    eval: LPrint("Print from Tangent Script")
```

##### 9. Include包含指令
Tangent建议将Include指令写在main函数最前端，同时无论Include写在何处，都会在解析时即时生效，不会出现执行到Include指令时才去解析包含文件的情况。两个文件互相访问是示例。必须注意的是，当脚本作为被include的对象，主函数不会被执行。
```properties

#文件名：Script_03.tt
include_test:
    $main:
        include: Script_04.tt
        eval: x = 3
        call: included_file.$func
        
#文件名：Script_04.tt
included_file:
    $main:
        log: 123
    $func:
        log: call from other script
        eval: LPrint(tostring(x))

```
