---
marp: true
theme: uncover
paginate: true
backgroundColor: #fff
backgroundImage: url('https://marp.app/assets/hero-background.svg')

---
# gql on .net with chilicream

![bg left:40% 80%](https://raw.githubusercontent.com/ChilliCream/hotchocolate/main/website/static/resources/hotchocolate-banner.svg)

---
# agenda

1. what is gql?
2. rest vs gql
3. basics of gql
4. chilicream and his toolset
5. coding time :D

---
### what is gql?

- is a declarative lenguage to fetch data from a server
- developed by facebook in 2012, realeased in 2015, open sourced in 2018
- to solve over and under fetching issues

---
### rest vs gql

| concept        | rest                  | gql           |
|----------------|-----------------------|---------------|
| endpoints      | one per entity        | /graphql      |
| fetch data     | GET                   | queries       |
| change data    | POST PUT PATCH DELETE | mutations     |

---
### rest vs gql

| concept        | rest       | gql           |
|----------------|------------|---------------|
| available data | api defined | api defined    |
| returned data  | api defined | client defined |
| type system    | no         | yes           |

---
### rest vs gql

| concept        | rest        | gql                  |
|----------------|-------------|----------------------|
| excute logic*  | controllers | resolvers            |
| transport      | http        | http, sockects, grcp | 
| web sockets    | -           | subscriptions        |

---
### basics of gql

[cheatsheet 📋](https://devhints.io/graphql)

---
## chilicream and hist toolset

- hot chocolate
- banana cake pop
- strawberry shake

---
# coding time :D

![](https://miro.medium.com/max/1400/0*oyD7ekV-hMU91h4J.png)