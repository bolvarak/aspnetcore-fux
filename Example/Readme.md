# `Fux` Documentation

---

## `TL;DR`
`Fux` is a .Net library targeting `net5.0`.  There are some things it does and some things it doesn't.
It also has a weirdly hilarious and slightly uncouth name.  Check out the examples here to figure out
how to use the bloody thing because I barely know and the documentation will never reflect it.
Thanks for coming to my TED talk.

---

[Overview](https://github.com/bolvarak/aspnetcore-fux-examples#Overview)
* [What the fux...](https://github.com/bolvarak/aspnetcore-fux-examples#What-the-fux...)

[Fux](https://github.com/bolvarak/aspnetcore-fux-examples#Fux)
* [Core](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Core)
    - [Attributes](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Core.Attribute)
    - [Extensions](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Core.Extension)
    - [Convert](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Core.Convert)
    - [Fork](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Core.Fork)
    - [Global](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Core.Global)
    - [Reflection](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Core.Reflection)
    - [Singleton](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Core.Singleton)
    - [Ticker](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Core.Ticker)
* [Config](https://github.com/bolvarak/aspnetcore-fux-examplesFux.Config)
    - [Abstractions](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Config.Abrstract)
    - [Attributes](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Config.Attribute)
    - [Docker](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Config.Docker)
    - [Environment](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Config.Environment)
    - [Redis](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Config.Redis)
* [Crypto](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Crypto)
* [Data](https://github.com/bolvarak/aspnetcore-fux-examplesFux.Data)
    - [Mongo](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Data.Mongo)
* [Dns](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Dns)
    - [Hostname](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Dns.Hostname)
* [Serve](https://github.com/bolvarak/aspnetcore-fux-examples#Fux.Serve)

---

### Overview

Keep an eye on this repository as it will be updated regularly as the
libraries are fleshed out to reflect all the new and exciting things
that have been added, changed and completely broken :speak_no_evil::hear_no_evil::see_no_evil:

For those of you looking at this document from [NuGet.org](https://nuget.org/)
rock on over to the [Github Repo](https://github.com/bolvarak/aspnetcore-fux-examples)
and take a look at the goodies there.

Happy coding home skillets :metal::metal:

---

#### What the fux...
We all know what it sounds like and in reality, it is.  However, being
the intellectual that I am, I like to think of `Fux` being the happy
median between fixed and absolutely fucked.

That's really how this library works.  Some things are [or will be] 
pretty awesome.  Then, other things will be absolutely terrible, broken
and just all around shite.  It's as if ADHD learned how to code and
decided to write a library :joy::joy:

---

### `Fux`
Okay, so on a serious note, Fux is a collection of things I've either found or created that became useful
and wanted to put them all in one place.  Fux probably makes some assumptions, each developer is different
after all, I did keep to this strict edge as much as possible to prevent generic and abstraction hell.  For
the most part Fux is the kind of library that you use to just do something.  It's mostly boilerplate to
alleviate the need to rewrite everything all the time in classic Microsoft fashion.  With that being said,
there are probably quite a few instances where I've reinvented the wheel.  If you see something like that,
lemme know or submit a pull request.  Join the party, it'll be fun!

---

#### `Fux.Core`
Core is really the heavy lifter of the library.  It's all about reflection, y'know, tearing shit apart and then
turning it into something else.  Also, *[singletons for days!](https://github.com/bolvarak/aspnetcore-fux-examples/Documentation/Core.md)*

| Source  | URL                                                                                                |
| ------- | -------------------------------------------------------------------------------------------------- |
| NuGet   | [https://www.nuget.org/packages/Fux.Core](https://www.nuget.org/packages/Fux.Core)                 |
| Github  | [https://github.com/bolvarak/aspnetcore-fux-core](https://github.com/bolvarak/aspnetcore-fux-core) |


---

##### `Fux.Core.Attribute`
I've said it before and I'll say it again:  What good is a library without some seriously
cool implementations of `System.Attribute`?!  Okay, maybe that's too much, but  yeah, you
know you wanna *[read more](https://github.com/bolvarak/aspnetcore-fux-examples/Documentation/Core/Attributes.md)*!

---

##### `Fux.Core.Extension`
Alright, who's ready to [`.FuxItUp()`](https://github.com/bolvarak/aspnetcore-fux-examples/Documentation/Core/Extensions.md), amirite?! :sweat_smile:
Unfortunately there is currently no such extension method, however I now feel obligated to make one.

---

##### `Fux.Core.Convert`
This bad boy takes objects of one type and converts them to another type.  We've got Expression selectors,
string names, the works.  You can [populate POCOs for days!](https://github.com/bolvarak/aspnetcore-fux-examples/Documentation/Core/Convert.md)

---

##### `Fux.Core.Fork`
This is one of those bits that I mentioned above that are terrible.  This is trash, disregard it, [or not.](https://github.com/bolvarak/aspnetcore-fux-examples/Documentation/Core/Fork.md)

---

##### `Fux.Core.Global`
I just needed a place to stash my `Newtonsoft.Json.JsonSerializerSettings`. I'm sure I can find other uses for it 

---

##### `Fux.Core.Reflection`
Anything and everything you never wanted to know about an object, right at your fingertips.  Not really, but it
is tightly coupled to `Convert` and `Singleton`.  This class provides the reverse mapping for expression property
mapping in `Convert`.  On top of that, every `Singleton` is actually a `Reflection<T>`.

---

##### `Fux.Core.Singleton`
This provides a lazy interface for instantiating objects as well as tracking single instances of things.
Just remember to use singletons wisely, they are not suited for everything, but they are efficient.

---

##### `Fux.Core.Ticker`
This is just a glorified `System.Diagnostics.Stopwatch`.  It will probably be modified to interact with `Fork`
to fire off tasks that need, or have the option to be, timed.

---

#### `Fux.Config`
This package basically provides a few wrappers and attributes for generating POCOs from values store in
Environment Variables, Docker Secrets and Redis Keys.  Currently, only string keys are supported in Redis.
If you send a hash to the library, it will convert it to a JSON array or object.

| Source  | URL                                                                                                    |
| ------- | ------------------------------------------------------------------------------------------------------ |
| NuGet   | [https://www.nuget.org/packages/Fux.Config](https://www.nuget.org/packages/Fux.Config)                 |
| Github  | [https://github.com/bolvarak/aspnetcore-fux-config](https://github.com/bolvarak/aspnetcore-fux-config) |

---

##### `Fux.Config.Abstract`
These are just implementable abstractions that help reduce active coding.  They also help maintain the structure
of the data and types flowing in and out of Fux.  The more it knows how to work with an object, the more efficient
it's going to be and ideally, the less code you end up writing.

---

##### `Fux.Config.Attribute`
Herein lies our respective property mapping attributes: `DockerSecretName`, `EnvironmentVariable`,
`RedisDatabase` and `RedisKey`.

---

##### `Fux.Config.Docker`
This class provides our interface into Docker, which is honestly just reading [from and trying to write to] secrets
in `/run/secrets`.  Add some typing magic in an there you have it.


##### `Fux.Config.Environment`
This class is the exact same as `Docker`, just that it reads [and writes to] from `System.Environment.GetEnvironmentVariable`.

##### `Fux.Config.Redis`
This class is a bit more complex as it has to establish an external connection which is ideally built from 
configuration variables itself.  Therefore bootstrapped classes have been added to establish a connection
with expected environment variables or docker secrets.  You can either use the default key and variable names,
which is what these examples use, or you can define your own POCOs and add the respective attribute(s).  You
can also build the connection the old fashioned way with long-form code and/or hard-coded values, but where's the fun in that?

---

#### `Fux.Crypto`
This namespace, which doesn't actually exist yet, will contain fluid cryptography.  Which is to say some random
encryption/decryption engine that reads and writes a portable hash that can be read with anything that supports
the generating algorithm and has the generating key.  Ooh, lets add in some redundant passes as well, kick up
those CPU cycles!

---

#### `Fux.Data`
Another namespace that doesn't actually exist yet, at least outside of my computer.  This behemoth will have
a ton of crap for working with SQL and Mongo databases, easily [dare I say, fluidly] allowing for custom SQL
and Mongo queries.

---

##### `Fux.Data.Mongo`
This has to be its own namespace because it attempts to emulate `EntityFrameworkCore`, but with Mongo, lol. 

| Source  | URL                                                                                                            |
| ------- | -------------------------------------------------------------------------------------------------------------- |
| NuGet   | [https://www.nuget.org/packages/Fux.Data.Mongo](https://www.nuget.org/packages/Fux.Data.Mongo)                 |
| Github  | [https://github.com/bolvarak/aspnetcore-fux-data-mongo](https://github.com/bolvarak/aspnetcore-fux-data-mongo) |

---

#### `Fux.Dns`
This namespace is kind of an outlier, it really only validates domain and host names against the PublicSuffix database.

| Source  | URL                                                                                              |
| ------- | ------------------------------------------------------------------------------------------------ |
| NuGet   | [https://www.nuget.org/packages/Fux.Dns](https://www.nuget.org/packages/Fux.Dns)                 |
| Github  | [https://github.com/bolvarak/aspnetcore-fux-dns](https://github.com/bolvarak/aspnetcore-fux-dns) |

---

##### `Fux.Dns.Hostname`
See above description.  It's a really really useful tool when you need it, but any other time it's pretty useless.

---

#### `Fux.Serve`
This namespace has [is going to have] all of the external reachability goodies.  You need an HTTP API?  Check!
A UNIX Socket listener?  Done!  Both?  Bro, we got you!

| Source  | URL                                                                                                  |
| ------- | ---------------------------------------------------------------------------------------------------- |
| NuGet   | [https://www.nuget.org/packages/Fux.Serve](https://www.nuget.org/packages/Fux.Serve)                 |
| Github  | [https://github.com/bolvarak/aspnetcore-fux-serve](https://github.com/bolvarak/aspnetcore-fux-serve) |
