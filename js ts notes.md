JavaScript & TypeScript Complete Tutorial
Table of Contents
JavaScript Basics
JavaScript Intermediate
JavaScript Advanced
TypeScript Basics
TypeScript Advanced
Best Practices
JavaScript Basics
1. Variables and Data Types
// Variable declarations
let age = 25;              // Block-scoped, reassignable
const name = "John";       // Block-scoped, constant
var oldWay = "avoid";      // Function-scoped (avoid using)

// Primitive Data Types
const string = "Hello";
const number = 42;
const boolean = true;
const nullValue = null;
const undefinedValue = undefined;
const symbol = Symbol("unique");
const bigInt = 9007199254740991n;

// Complex Data Types
const array = [1, 2, 3];
const object = { key: "value" };
Best Practice: Use const by default, let when reassignment is needed, avoid var.
2. Operators
// Arithmetic
let sum = 5 + 3;           // 8
let difference = 10 - 4;   // 6
let product = 4 * 5;       // 20
let quotient = 20 / 4;     // 5
let remainder = 10 % 3;    // 1
let power = 2 ** 3;        // 8

// Comparison
5 === 5;                   // true (strict equality)
5 == "5";                  // true (loose equality, avoid)
5 !== 6;                   // true
10 > 5;                    // true
10 >= 10;                  // true

// Logical
true && false;             // false (AND)
true || false;             // true (OR)
!true;                     // false (NOT)

// Nullish Coalescing
const value = null ?? "default";  // "default"
const value2 = 0 ?? "default";    // 0

// Optional Chaining
const user = { address: { city: "NYC" } };
const city = user?.address?.city;  // "NYC"
const missing = user?.phone?.number;  // undefined
3. Control Flow
// If-Else
const temperature = 75;
if (temperature > 80) {
  console.log("Hot");
} else if (temperature > 60) {
  console.log("Nice");
} else {
  console.log("Cold");
}

// Ternary Operator
const status = age >= 18 ? "adult" : "minor";

// Switch Statement
const day = "Monday";
switch (day) {
  case "Monday":
    console.log("Start of week");
    break;
  case "Friday":
    console.log("Almost weekend");
    break;
  default:
    console.log("Regular day");
}
4. Loops
// For Loop
for (let i = 0; i < 5; i++) {
  console.log(i);  // 0, 1, 2, 3, 4
}

// While Loop
let count = 0;
while (count < 3) {
  console.log(count);
  count++;
}

// Do-While Loop
let num = 0;
do {
  console.log(num);
  num++;
} while (num < 3);

// For...of (iterate over values)
const fruits = ["apple", "banana", "orange"];
for (const fruit of fruits) {
  console.log(fruit);
}

// For...in (iterate over keys, avoid for arrays)
const person = { name: "Alice", age: 30 };
for (const key in person) {
  console.log(`${key}: ${person[key]}`);
}
5. Functions
// Function Declaration
function greet(name) {
  return `Hello, ${name}!`;
}

// Function Expression
const greet2 = function(name) {
  return `Hello, ${name}!`;
};

// Arrow Functions
const greet3 = (name) => `Hello, ${name}!`;
const add = (a, b) => a + b;
const multiply = (x, y) => {
  const result = x * y;
  return result;
};

// Default Parameters
function createUser(name, role = "user") {
  return { name, role };
}

// Rest Parameters
function sum(...numbers) {
  return numbers.reduce((total, num) => total + num, 0);
}
console.log(sum(1, 2, 3, 4));  // 10

// Destructuring Parameters
function printUser({ name, age }) {
  console.log(`${name} is ${age} years old`);
}
printUser({ name: "Bob", age: 25 });
6. Arrays
const numbers = [1, 2, 3, 4, 5];

// Common Methods
numbers.push(6);           // Add to end
numbers.pop();             // Remove from end
numbers.unshift(0);        // Add to beginning
numbers.shift();           // Remove from beginning
numbers.includes(3);       // true
numbers.indexOf(3);        // 2
numbers.slice(1, 3);       // [2, 3] (doesn't modify original)
numbers.splice(1, 2);      // Removes 2 elements at index 1

// Iteration Methods
numbers.forEach(num => console.log(num));

const doubled = numbers.map(num => num * 2);
const evens = numbers.filter(num => num % 2 === 0);
const sum = numbers.reduce((acc, num) => acc + num, 0);
const hasEven = numbers.some(num => num % 2 === 0);
const allPositive = numbers.every(num => num > 0);
const firstEven = numbers.find(num => num % 2 === 0);
const firstEvenIndex = numbers.findIndex(num => num % 2 === 0);

// Spreading
const morNumbers = [...numbers, 6, 7, 8];
const combined = [...numbers, ...morNumbers];
7. Objects
// Object Creation
const user = {
  name: "Alice",
  age: 30,
  email: "alice@example.com",
  greet() {
    return `Hi, I'm ${this.name}`;
  }
};

// Accessing Properties
user.name;              // Dot notation
user["email"];          // Bracket notation
const key = "age";
user[key];              // Dynamic access

// Adding/Modifying Properties
user.city = "NYC";
user.age = 31;

// Deleting Properties
delete user.email;

// Object Methods
Object.keys(user);      // ["name", "age", "city", "greet"]
Object.values(user);    // ["Alice", 31, "NYC", function]
Object.entries(user);   // [["name", "Alice"], ["age", 31], ...]

// Object Spreading
const updatedUser = { ...user, age: 32, country: "USA" };

// Destructuring
const { name, age } = user;
const { name: userName, age: userAge = 18 } = user;

// Shorthand Property Names
const x = 10, y = 20;
const point = { x, y };  // { x: 10, y: 20 }

// Computed Property Names
const prop = "dynamic";
const obj = {
  [prop]: "value",
  [`${prop}Key`]: "another value"
};
JavaScript Intermediate
8. Advanced Functions
// Closures
function createCounter() {
  let count = 0;
  return {
    increment() { return ++count; },
    decrement() { return --count; },
    getCount() { return count; }
  };
}
const counter = createCounter();
counter.increment();  // 1
counter.increment();  // 2

// IIFE (Immediately Invoked Function Expression)
(function() {
  console.log("Running immediately");
})();

// Higher-Order Functions
function withLogging(fn) {
  return function(...args) {
    console.log(`Calling with args: ${args}`);
    return fn(...args);
  };
}
const loggedAdd = withLogging((a, b) => a + b);

// Currying
const curry = (fn) => {
  return function curried(...args) {
    if (args.length >= fn.length) {
      return fn(...args);
    }
    return (...moreArgs) => curried(...args, ...moreArgs);
  };
};
const add3 = (a, b, c) => a + b + c;
const curriedAdd = curry(add3);
curriedAdd(1)(2)(3);  // 6
9. Asynchronous JavaScript
// Callbacks
function fetchData(callback) {
  setTimeout(() => {
    callback({ data: "result" });
  }, 1000);
}

// Promises
const promise = new Promise((resolve, reject) => {
  setTimeout(() => {
    const success = true;
    if (success) {
      resolve("Success!");
    } else {
      reject("Error!");
    }
  }, 1000);
});

promise
  .then(result => console.log(result))
  .catch(error => console.error(error))
  .finally(() => console.log("Done"));

// Promise Chaining
fetch("https://api.example.com/data")
  .then(response => response.json())
  .then(data => processData(data))
  .then(result => console.log(result))
  .catch(error => console.error(error));

// Async/Await
async function fetchUserData() {
  try {
    const response = await fetch("https://api.example.com/user");
    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error fetching data:", error);
    throw error;
  }
}

// Parallel Execution
async function fetchMultiple() {
  const [users, posts, comments] = await Promise.all([
    fetch("/api/users").then(r => r.json()),
    fetch("/api/posts").then(r => r.json()),
    fetch("/api/comments").then(r => r.json())
  ]);
  return { users, posts, comments };
}

// Promise.race
const timeout = (ms) => new Promise((_, reject) => 
  setTimeout(() => reject(new Error("Timeout")), ms)
);

async function fetchWithTimeout(url, ms) {
  return Promise.race([
    fetch(url),
    timeout(ms)
  ]);
}
10. Error Handling
// Try-Catch
try {
  const result = riskyOperation();
  console.log(result);
} catch (error) {
  console.error("An error occurred:", error.message);
} finally {
  console.log("Cleanup code here");
}

// Custom Errors
class ValidationError extends Error {
  constructor(message) {
    super(message);
    this.name = "ValidationError";
  }
}

function validateAge(age) {
  if (age < 0) {
    throw new ValidationError("Age cannot be negative");
  }
  return true;
}

// Error handling with async/await
async function safeOperation() {
  try {
    const data = await fetchData();
    return data;
  } catch (error) {
    if (error instanceof ValidationError) {
      console.error("Validation failed:", error.message);
    } else {
      console.error("Unexpected error:", error);
    }
    throw error;
  }
}
11. Classes and OOP
// Class Definition
class Animal {
  constructor(name, age) {
    this.name = name;
    this.age = age;
  }
  
  speak() {
    console.log(`${this.name} makes a sound`);
  }
  
  // Getter
  get info() {
    return `${this.name} is ${this.age} years old`;
  }
  
  // Setter
  set age(value) {
    if (value < 0) throw new Error("Age must be positive");
    this._age = value;
  }
  
  // Static method
  static create(name, age) {
    return new Animal(name, age);
  }
}

// Inheritance
class Dog extends Animal {
  constructor(name, age, breed) {
    super(name, age);  // Call parent constructor
    this.breed = breed;
  }
  
  speak() {
    console.log(`${this.name} barks`);
  }
  
  fetch() {
    console.log(`${this.name} fetches the ball`);
  }
}

const dog = new Dog("Rex", 5, "Labrador");
dog.speak();  // "Rex barks"

// Private Fields (Modern JS)
class BankAccount {
  #balance = 0;  // Private field
  
  constructor(initialBalance) {
    this.#balance = initialBalance;
  }
  
  deposit(amount) {
    this.#balance += amount;
  }
  
  getBalance() {
    return this.#balance;
  }
}
12. Modules
// Exporting (file: math.js)
export const PI = 3.14159;
export function add(a, b) { return a + b; }
export default class Calculator {
  multiply(a, b) { return a * b; }
}

// Importing
import Calculator, { PI, add } from './math.js';
import * as MathUtils from './math.js';

// Dynamic Imports
async function loadModule() {
  const module = await import('./module.js');
  module.doSomething();
}
JavaScript Advanced
13. Prototypes and Inheritance
// Prototype Chain
function Person(name) {
  this.name = name;
}

Person.prototype.greet = function() {
  return `Hello, I'm ${this.name}`;
};

const john = new Person("John");
console.log(john.greet());  // "Hello, I'm John"

// Prototype Inheritance
function Employee(name, title) {
  Person.call(this, name);
  this.title = title;
}

Employee.prototype = Object.create(Person.prototype);
Employee.prototype.constructor = Employee;

Employee.prototype.work = function() {
  return `${this.name} is working as ${this.title}`;
};

// Object.create
const personPrototype = {
  greet() {
    return `Hi, I'm ${this.name}`;
  }
};

const alice = Object.create(personPrototype);
alice.name = "Alice";
14. Iterators and Generators
// Custom Iterator
const range = {
  from: 1,
  to: 5,
  [Symbol.iterator]() {
    let current = this.from;
    const last = this.to;
    return {
      next() {
        if (current <= last) {
          return { value: current++, done: false };
        }
        return { done: true };
      }
    };
  }
};

for (const num of range) {
  console.log(num);  // 1, 2, 3, 4, 5
}

// Generators
function* numberGenerator() {
  yield 1;
  yield 2;
  yield 3;
}

const gen = numberGenerator();
console.log(gen.next());  // { value: 1, done: false }
console.log(gen.next());  // { value: 2, done: false }

// Infinite Generator
function* fibonacci() {
  let [prev, curr] = [0, 1];
  while (true) {
    yield curr;
    [prev, curr] = [curr, prev + curr];
  }
}

const fib = fibonacci();
for (let i = 0; i < 10; i++) {
  console.log(fib.next().value);
}

// Async Generators
async function* fetchPages(urls) {
  for (const url of urls) {
    const response = await fetch(url);
    yield await response.json();
  }
}
15. Proxies and Reflection
// Proxy for Validation
const validator = {
  set(target, property, value) {
    if (property === "age" && typeof value !== "number") {
      throw new TypeError("Age must be a number");
    }
    target[property] = value;
    return true;
  }
};

const person = new Proxy({}, validator);
person.age = 30;  // OK
// person.age = "thirty";  // TypeError

// Proxy for Logging
const handler = {
  get(target, property) {
    console.log(`Getting ${property}`);
    return target[property];
  },
  set(target, property, value) {
    console.log(`Setting ${property} to ${value}`);
    target[property] = value;
    return true;
  }
};

const obj = new Proxy({ name: "Alice" }, handler);

// Reflect API
const myObj = { x: 1, y: 2 };
Reflect.has(myObj, "x");           // true
Reflect.get(myObj, "x");           // 1
Reflect.set(myObj, "z", 3);        // true
Reflect.deleteProperty(myObj, "x"); // true
16. Advanced Array Methods
const users = [
  { name: "Alice", age: 25, active: true },
  { name: "Bob", age: 30, active: false },
  { name: "Charlie", age: 35, active: true }
];

// FlatMap
const nested = [[1, 2], [3, 4], [5, 6]];
nested.flatMap(arr => arr);  // [1, 2, 3, 4, 5, 6]

// Group By (Object.groupBy)
const grouped = users.reduce((acc, user) => {
  const key = user.active ? "active" : "inactive";
  (acc[key] = acc[key] || []).push(user);
  return acc;
}, {});

// Array.from with mapping
Array.from({ length: 5 }, (_, i) => i * 2);  // [0, 2, 4, 6, 8]

// at() method
const arr = [10, 20, 30];
arr.at(-1);  // 30 (last element)

// Sorting with custom comparator
const sorted = users.sort((a, b) => a.age - b.age);
17. WeakMap and WeakSet
// WeakMap (keys must be objects, allows garbage collection)
const privateData = new WeakMap();

class User {
  constructor(name) {
    privateData.set(this, { password: "secret" });
    this.name = name;
  }
  
  getPassword() {
    return privateData.get(this).password;
  }
}

// WeakSet
const visitedNodes = new WeakSet();

function traverse(node) {
  if (visitedNodes.has(node)) return;
  visitedNodes.add(node);
  // Process node...
}
18. Regular Expressions
// Basic Patterns
const emailPattern = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/;
const phonePattern = /^\d{3}-\d{3}-\d{4}$/;

// Methods
const text = "Hello World";
text.match(/o/g);           // ["o", "o"]
text.search(/World/);       // 6
text.replace(/World/, "JS"); // "Hello JS"
text.split(/\s/);           // ["Hello", "World"]

// Test
/\d+/.test("123");          // true

// Groups
const date = "2024-01-15";
const pattern = /(\d{4})-(\d{2})-(\d{2})/;
const [_, year, month, day] = date.match(pattern);

// Named Groups
const urlPattern = /(?<protocol>https?):\/\/(?<domain>[^\/]+)/;
const match = "https://example.com".match(urlPattern);
console.log(match.groups.domain);  // "example.com"
TypeScript Basics
19. Type Annotations
// Basic Types
let name: string = "Alice";
let age: number = 30;
let isActive: boolean = true;
let nothing: null = null;
let notDefined: undefined = undefined;

// Arrays
let numbers: number[] = [1, 2, 3];
let strings: Array<string> = ["a", "b", "c"];

// Tuples
let tuple: [string, number] = ["Alice", 30];
let tuple2: [string, number, boolean?] = ["Bob", 25];

// Any (avoid when possible)
let anything: any = "could be anything";

// Unknown (safer than any)
let userInput: unknown = getUserInput();
if (typeof userInput === "string") {
  console.log(userInput.toUpperCase());
}

// Void
function logMessage(message: string): void {
  console.log(message);
}

// Never
function throwError(message: string): never {
  throw new Error(message);
}

// Object
let user: { name: string; age: number } = {
  name: "Alice",
  age: 30
};
20. Interfaces and Types
// Interface
interface User {
  id: number;
  name: string;
  email: string;
  age?: number;              // Optional property
  readonly createdAt: Date;  // Readonly property
}

const user: User = {
  id: 1,
  name: "Alice",
  email: "alice@example.com",
  createdAt: new Date()
};

// Interface Extension
interface Admin extends User {
  role: string;
  permissions: string[];
}

// Type Alias
type ID = string | number;
type Point = { x: number; y: number };
type Status = "pending" | "approved" | "rejected";

// Union Types
let id: string | number = 123;
id = "ABC123";

// Intersection Types
type Employee = User & { department: string };

// Function Types
type MathOperation = (a: number, b: number) => number;
const add: MathOperation = (a, b) => a + b;

// Interface vs Type
// Interfaces can be extended/merged, types cannot
interface Window {
  title: string;
}
interface Window {
  ts: number;
}
// Results in: { title: string; ts: number }
21. Functions in TypeScript
// Function with type annotations
function greet(name: string): string {
  return `Hello, ${name}`;
}

// Optional parameters
function buildName(first: string, last?: string): string {
  return last ? `${first} ${last}` : first;
}

// Default parameters
function createUser(name: string, role: string = "user"): User {
  return { name, role };
}

// Rest parameters
function sum(...numbers: number[]): number {
  return numbers.reduce((acc, n) => acc + n, 0);
}

// Function overloads
function process(value: string): string;
function process(value: number): number;
function process(value: string | number): string | number {
  if (typeof value === "string") {
    return value.toUpperCase();
  }
  return value * 2;
}

// Arrow function types
const multiply: (a: number, b: number) => number = (a, b) => a * b;

// Callback types
function fetchData(callback: (data: string) => void): void {
  callback("data");
}
22. Classes in TypeScript
class Person {
  // Property declarations
  private name: string;
  protected age: number;
  public email: string;
  readonly id: number;
  
  // Constructor with parameter properties
  constructor(
    name: string,
    age: number,
    public city: string  // Shorthand for declaration + assignment
  ) {
    this.name = name;
    this.age = age;
    this.id = Date.now();
  }
  
  // Methods
  greet(): string {
    return `Hello, I'm ${this.name}`;
  }
  
  // Getter
  get info(): string {
    return `${this.name}, ${this.age}`;
  }
  
  // Setter
  set updateAge(value: number) {
    if (value > 0) {
      this.age = value;
    }
  }
  
  // Static members
  static species: string = "Homo sapiens";
  
  static create(name: string, age: number, city: string): Person {
    return new Person(name, age, city);
  }
}

// Abstract classes
abstract class Animal {
  abstract makeSound(): void;
  
  move(): void {
    console.log("Moving...");
  }
}

class Dog extends Animal {
  makeSound(): void {
    console.log("Woof!");
  }
}

// Implementing interfaces
interface Printable {
  print(): void;
}

class Document implements Printable {
  print(): void {
    console.log("Printing document");
  }
}
TypeScript Advanced
23. Generics
// Generic Function
function identity<T>(arg: T): T {
  return arg;
}

const num = identity<number>(42);
const str = identity("hello");  // Type inference

// Generic Interface
interface Box<T> {
  value: T;
}

const numberBox: Box<number> = { value: 42 };
const stringBox: Box<string> = { value: "hello" };

// Generic Class
class GenericStorage<T> {
  private items: T[] = [];
  
  add(item: T): void {
    this.items.push(item);
  }
  
  getAll(): T[] {
    return this.items;
  }
}

const numberStorage = new GenericStorage<number>();
numberStorage.add(1);

// Generic Constraints
interface HasLength {
  length: number;
}

function logLength<T extends HasLength>(arg: T): void {
  console.log(arg.length);
}

logLength("hello");      // OK
logLength([1, 2, 3]);    // OK
// logLength(42);        // Error: number doesn't have length

// Multiple Type Parameters
function merge<T, U>(obj1: T, obj2: U): T & U {
  return { ...obj1, ...obj2 };
}

const merged = merge({ name: "Alice" }, { age: 30 });

// Generic with default types
interface Response<T = any> {
  data: T;
  status: number;
}
24.
24. Utility Types
interface User {
  id: number;
  name: string;
  email: string;
  age: number;
}

// Partial - makes all properties optional
type PartialUser = Partial<User>;
const update: PartialUser = { name: "Alice" };

// Required - makes all properties required
type RequiredUser = Required<PartialUser>;

// Readonly - makes all properties readonly
type ReadonlyUser = Readonly<User>;

// Pick - select specific properties
type UserPreview = Pick<User, "id" | "name">;

// Omit - exclude specific properties
type UserWithoutEmail = Omit<User, "email">;

// Record - create object type with specific keys and value type
type Roles = "admin" | "user" | "guest";
type RolePermissions = Record<Roles, string[]>;

const permissions: RolePermissions = {
  admin: ["read", "write", "delete"],
  user: ["read", "write"],
  guest: ["read"]
};

// Exclude - exclude types from union
type T1 = Exclude<"a" | "b" | "c", "a">;  // "b" | "c"

// Extract - extract types from union
type T2 = Extract<"a" | "b" | "c", "a" | "f">;  // "a"

// NonNullable - remove null and undefined
type T3 = NonNullable<string | number | null>;  // string | number

// ReturnType - get return type of function
function createUser() {
  return { id: 1, name: "Alice" };
}
type UserReturn = ReturnType<typeof createUser>;

// Parameters - get parameter types as tuple
type CreateUserParams = Parameters<typeof createUser>;

// Awaited - unwrap Promise type
type AwaitedUser = Awaited<Promise<User>>;  // User
25. Advanced Types
// Mapped Types
type ReadonlyProps<T> = {
  readonly [K in keyof T]: T[K];
};

type OptionalProps<T> = {
  [K in keyof T]?: T[K];
};

// Conditional Types
type IsString<T> = T extends string ? true : false;
type A = IsString<string>;  // true
type B = IsString<number>;  // false

// Infer keyword
type ReturnTypeCustom<T> = T extends (...args: any[]) => infer R ? R : never;

type ArrayElement<T> = T extends (infer E)[] ? E : never;
type StringArray = ArrayElement<string[]>;  // string

// Template Literal Types
type EventName = "click" | "scroll" | "mousemove";
type EventHandler = `on${Capitalize<EventName>}`;
// "onClick" | "onScroll" | "onMousemove"

// Discriminated Unions
type Success = { status: "success"; data: string };
type Error = { status: "error"; error: string };
type Loading = { status: "loading" };
type State = Success | Error | Loading;

function handleState(state: State) {
  switch (state.status) {
    case "success":
      console.log(state.data);
      break;
    case "error":
      console.log(state.error);
      break;
    case "loading":
      console.log("Loading...");
      break;
  }
}

// Type Guards
function isString(value: unknown): value is string {
  return typeof value === "string";
}

function processValue(value: string | number) {
  if (isString(value)) {
    console.log(value.toUpperCase());
  } else {
    console.log(value.toFixed(2));
  }
}

// Index Signatures
interface StringMap {
  [key: string]: string;
}

const colors: StringMap = {
  red: "#FF0000",
  blue: "#0000FF"
};
26. Decorators (Experimental)
// Class Decorator
function sealed(constructor: Function) {
  Object.seal(constructor);
  Object.seal(constructor.prototype);
}

@sealed
class BugReport {
  type = "report";
  title: string;
  
  constructor(t: string) {
    this.title = t;
  }
}

// Method Decorator
function log(target: any, propertyKey: string, descriptor: PropertyDescriptor) {
  const originalMethod = descriptor.value;
  
  descriptor.value = function(...args: any[]) {
    console.log(`Calling ${propertyKey} with`, args);
    return originalMethod.apply(this, args);
  };
}

class Calculator {
  @log
  add(a: number, b: number): number {
    return a + b;
  }
}

// Property Decorator
function readonly(target: any, propertyKey: string) {
  Object.defineProperty(target, propertyKey, {
    writable: false
  });
}

class Person {
  @readonly
  name: string = "Alice";
}

// Parameter Decorator
function required(target: any, propertyKey: string, parameterIndex: number) {
  console.log(`Parameter at index ${parameterIndex} in ${propertyKey} is required`);
}

class Greeter {
  greet(@required name: string) {
    return `Hello, ${name}`;
  }
}
27. Namespaces and Modules
// Namespace
namespace Validation {
  export interface StringValidator {
    isValid(s: string): boolean;
  }
  
  export class EmailValidator implements StringValidator {
    isValid(s: string): boolean {
      return /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(s);
    }
  }
  
  export class ZipCodeValidator implements StringValidator {
    isValid(s: string): boolean {
      return /^\d{5}$/.test(s);
    }
  }
}

const emailValidator = new Validation.EmailValidator();

// Triple-Slash Directives (for references)
/// <reference path="validation.ts" />

// Module Augmentation
// Extending existing module
declare module "express" {
  interface Request {
    user?: User;
  }
}
28. Declaration Files
// types.d.ts - Type declarations for external libraries

// Declare module
declare module "my-library" {
  export function doSomething(value: string): void;
  export const version: string;
}

// Declare global
declare global {
  interface Window {
    myCustomProperty: string;
  }
  
  const MY_GLOBAL: string;
}

// Ambient declarations
declare const jQuery: (selector: string) => any;
declare namespace NodeJS {
  interface ProcessEnv {
    NODE_ENV: "development" | "production";
    API_KEY: string;
  }
}
29. Strict Type Checking
// Enable in tsconfig.json:
// "strict": true

// strictNullChecks
let name: string;
// name = null;  // Error with strictNullChecks

let maybeName: string | null = null;  // Explicitly allow null

// strictFunctionTypes
type Handler = (value: string | number) => void;
type StringHandler = (value: string) => void;

// let handler: Handler = (value: string) => {};  // Error

// noImplicitAny
function log(message) {  // Error: Parameter implicitly has 'any' type
  console.log(message);
}

// strictPropertyInitialization
class User {
  name: string;  // Error: Property has no initializer
  age: number = 0;  // OK
  email!: string;  // OK with definite assignment assertion
  
  constructor() {
    this.name = "Unknown";
  }
}

// noImplicitThis
function logThis() {
  console.log(this);  // Error: 'this' implicitly has type 'any'
}

// Proper this typing
interface MyObject {
  value: number;
  getValue(this: MyObject): number;
}

const obj: MyObject = {
  value: 42,
  getValue() {
    return this.value;
  }
};
Best Practices
30. Code Organization
// Single Responsibility Principle
// ❌ Bad
class UserManager {
  validateEmail(email: string) {}
  saveToDatabase(user: User) {}
  sendEmail(email: string) {}
}

// ✅ Good
class EmailValidator {
  validate(email: string): boolean {}
}

class UserRepository {
  save(user: User): Promise<void> {}
}

class EmailService {
  send(email: string, content: string): Promise<void> {}
}

// Dependency Injection
interface ILogger {
  log(message: string): void;
}

class ConsoleLogger implements ILogger {
  log(message: string): void {
    console.log(message);
  }
}

class UserService {
  constructor(private logger: ILogger) {}
  
  createUser(user: User): void {
    this.logger.log(`Creating user: ${user.name}`);
    // Create user logic
  }
}

const logger = new ConsoleLogger();
const userService = new UserService(logger);
31. Error Handling Best Practices
// Custom Error Classes
class ValidationError extends Error {
  constructor(
    message: string,
    public field: string
  ) {
    super(message);
    this.name = "ValidationError";
    Object.setPrototypeOf(this, ValidationError.prototype);
  }
}

class NotFoundError extends Error {
  constructor(
    message: string,
    public resource: string
  ) {
    super(message);
    this.name = "NotFoundError";
    Object.setPrototypeOf(this, NotFoundError.prototype);
  }
}

// Result Type Pattern
type Result<T, E = Error> = 
  | { success: true; value: T }
  | { success: false; error: E };

async function fetchUser(id: string): Promise<Result<User>> {
  try {
    const user = await api.getUser(id);
    return { success: true, value: user };
  } catch (error) {
    return { 
      success: false, 
      error: error instanceof Error ? error : new Error("Unknown error")
    };
  }
}

// Usage
const result = await fetchUser("123");
if (result.success) {
  console.log(result.value.name);
} else {
  console.error(result.error.message);
}

// Error Boundary Pattern (React)
class ErrorBoundary extends React.Component<
  { children: React.ReactNode },
  { hasError: boolean }
> {
  state = { hasError: false };
  
  static getDerivedStateFromError(error: Error) {
    return { hasError: true };
  }
  
  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    console.error("Error caught:", error, errorInfo);
  }
  
  render() {
    if (this.state.hasError) {
      return <h1>Something went wrong.</h1>;
    }
    return this.props.children;
  }
}
32. Performance Best Practices
// Debouncing
function debounce<T extends (...args: any[]) => any>(
  func: T,
  wait: number
): (...args: Parameters<T>) => void {
  let timeout: NodeJS.Timeout;
  
  return function(...args: Parameters<T>) {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
}

const searchHandler = debounce((query: string) => {
  console.log("Searching for:", query);
}, 300);

// Throttling
function throttle<T extends (...args: any[]) => any>(
  func: T,
  limit: number
): (...args: Parameters<T>) => void {
  let inThrottle: boolean;
  
  return function(...args: Parameters<T>) {
    if (!inThrottle) {
      func(...args);
      inThrottle = true;
      setTimeout(() => inThrottle = false, limit);
    }
  };
}

const scrollHandler = throttle(() => {
  console.log("Scroll event");
}, 100);

// Memoization
function memoize<T extends (...args: any[]) => any>(fn: T): T {
  const cache = new Map();
  
  return ((...args: any[]) => {
    const key = JSON.stringify(args);
    if (cache.has(key)) {
      return cache.get(key);
    }
    const result = fn(...args);
    cache.set(key, result);
    return result;
  }) as T;
}

const expensiveCalculation = memoize((n: number): number => {
  console.log("Calculating...");
  return n * n;
});

// Lazy Loading
const LazyComponent = React.lazy(() => import('./HeavyComponent'));

function App() {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <LazyComponent />
    </Suspense>
  );
}

// Virtual Scrolling concept
function renderVisibleItems(
  items: any[],
  containerHeight: number,
  itemHeight: number,
  scrollTop: number
) {
  const startIndex = Math.floor(scrollTop / itemHeight);
  const endIndex = Math.ceil((scrollTop + containerHeight) / itemHeight);
  return items.slice(startIndex, endIndex);
}
33. Design Patterns
// Singleton Pattern
class Database {
  private static instance: Database;
  private constructor() {}
  
  static getInstance(): Database {
    if (!Database.instance) {
      Database.instance = new Database();
    }
    return Database.instance;
  }
  
  query(sql: string): void {
    console.log(`Executing: ${sql}`);
  }
}

const db1 = Database.getInstance();
const db2 = Database.getInstance();
console.log(db1 === db2);  // true

// Factory Pattern
interface Animal {
  speak(): void;
}

class Dog implements Animal {
  speak() { console.log("Woof!"); }
}

class Cat implements Animal {
  speak() { console.log("Meow!"); }
}

class AnimalFactory {
  static create(type: "dog" | "cat"): Animal {
    switch (type) {
      case "dog": return new Dog();
      case "cat": return new Cat();
    }
  }
}

const animal = AnimalFactory.create("dog");

// Observer Pattern
interface Observer {
  update(data: any): void;
}

class Subject {
  private observers: Observer[] = [];
  
  subscribe(observer: Observer): void {
    this.observers.push(observer);
  }
  
  unsubscribe(observer: Observer): void {
    const index = this.observers.indexOf(observer);
    if (index > -1) {
      this.observers.splice(index, 1);
    }
  }
  
  notify(data: any): void {
    this.observers.forEach(observer => observer.update(data));
  }
}

class ConcreteObserver implements Observer {
  constructor(private name: string) {}
  
  update(data: any): void {
    console.log(`${this.name} received:`, data);
  }
}

// Strategy Pattern
interface SortStrategy {
  sort(data: number[]): number[];
}

class BubbleSort implements SortStrategy {
  sort(data: number[]): number[] {
    // Bubble sort implementation
    return data;
  }
}

class QuickSort implements SortStrategy {
  sort(data: number[]): number[] {
    // Quick sort implementation
    return data;
  }
}

class Sorter {
  constructor(private strategy: SortStrategy) {}
  
  setStrategy(strategy: SortStrategy): void {
    this.strategy = strategy;
  }
  
  sort(data: number[]): number[] {
    return this.strategy.sort(data);
  }
}

// Builder Pattern
class UserBuilder {
  private user: Partial<User> = {};
  
  setName(name: string): this {
    this.user.name = name;
    return this;
  }
  
  setEmail(email: string): this {
    this.user.email = email;
    return this;
  }
  
  setAge(age: number): this {
    this.user.age = age;
    return this;
  }
  
  build(): User {
    if (!this.user.name || !this.user.email) {
      throw new Error("Name and email are required");
    }
    return this.user as User;
  }
}

const user = new UserBuilder()
  .setName("Alice")
  .setEmail("alice@example.com")
  .setAge(30)
  .build();
34. Testing Best Practices
// Unit Testing with Jest
describe("Calculator", () => {
  let calculator: Calculator;
  
  beforeEach(() => {
    calculator = new Calculator();
  });
  
  describe("add", () => {
    it("should add two positive numbers", () => {
      expect(calculator.add(2, 3)).toBe(5);
    });
    
    it("should handle negative numbers", () => {
      expect(calculator.add(-2, 3)).toBe(1);
    });
  });
  
  describe("divide", () => {
    it("should throw error when dividing by zero", () => {
      expect(() => calculator.divide(10, 0)).toThrow("Cannot divide by zero");
    });
  });
});

// Mocking
jest.mock("./userService");

describe("UserController", () => {
  it("should fetch user data", async () => {
    const mockUser = { id: 1, name: "Alice" };
    (UserService.getUser as jest.Mock).mockResolvedValue(mockUser);
    
    const controller = new UserController();
    const result = await controller.getUser(1);
    
    expect(result).toEqual(mockUser);
    expect(UserService.getUser).toHaveBeenCalledWith(1);
  });
});

// Testing Async Code
describe("fetchData", () => {
  it("should fetch data successfully", async () => {
    const data = await fetchData();
    expect(data).toBeDefined();
  });
  
  it("should handle errors", async () => {
    await expect(fetchInvalidData()).rejects.toThrow("Error");
  });
});

// Snapshot Testing
describe("Component", () => {
  it("should render correctly", () => {
    const tree = renderer.create(<MyComponent />).toJSON();
    expect(tree).toMatchSnapshot();
  });
});
35. Code Quality Guidelines
// ✅ Good Practices

// 1. Use meaningful names
const getUserById = (id: string) => {};  // Good
const gub = (i: string) => {};           // Bad

// 2. Keep functions small and focused
function createUser(userData: UserData): User {
  validateUserData(userData);
  const user = buildUserObject(userData);
  saveUser(user);
  sendWelcomeEmail(user.email);
  return user;
}

// 3. Avoid deep nesting
// ❌ Bad
function processData(data: any) {
  if (data) {
    if (data.items) {
      if (data.items.length > 0) {
        // Process...
      }
    }
  }
}

// ✅ Good
function processData(data: any) {
  if (!data?.items?.length) return;
  // Process...
}

// 4. Use const for immutability
const config = { apiUrl: "https://api.example.com" };
const users = [...existingUsers, newUser];

// 5. Proper type annotations
interface UserService {
  getUser(id: string): Promise<User>;
  createUser(data: CreateUserDto): Promise<User>;
  updateUser(id: string, data: UpdateUserDto): Promise<User>;
}

// 6. Avoid magic numbers
const MAX_RETRY_ATTEMPTS = 3;
const TIMEOUT_MS = 5000;

// 7. Use enums for constants
enum UserRole {
  Admin = "ADMIN",
  User = "USER",
  Guest = "GUEST"
}

// 8. Document complex logic
/**
 * Calculates the compound interest
 * @param principal - Initial amount
 * @param rate - Annual interest rate (as decimal)
 * @param time - Time period in years
 * @param frequency - Compounding frequency per year
 * @returns The final amount
 */
function calculateCompoundInterest(
  principal: number,
  rate: number,
  time: number,
  frequency: number
): number {
  return principal * Math.pow(1 + rate / frequency, frequency * time);
}

// 9. Handle edge cases
function divide(a: number, b: number): number {
  if (b === 0) throw new Error("Cannot divide by zero");
  if (!Number.isFinite(a) || !Number.isFinite(b)) {
    throw new Error("Arguments must be finite numbers");
  }
  return a / b;
}

// 10. Use optional chaining and nullish coalescing
const username = user?.profile?.name ?? "Anonymous";
const port = process.env.PORT ?? 3000;
36. Modern JavaScript/TypeScript Features
// Optional Chaining
const city = user?.address?.city;
const result = obj.method?.();
const item = arr?.[0];

// Nullish Coalescing
const value = input ?? defaultValue;
const port = config.port ?? 3000;

// Logical Assignment Operators
let x = 1;
x ||= 10;  // x = x || 10
x &&= 5;   // x = x && 5
x ??= 20;  // x = x ?? 20

// Numeric Separators
const billion = 1_000_000_000;
const hex = 0xFF_FF_FF;

// Private Class Fields
class BankAccount {
  #balance = 0;
  
  deposit(amount: number) {
    this.#balance += amount;
  }
  
  getBalance() {
    return this.#balance;
  }
}

// Top-level await
const data = await fetch("/api/data").then(r => r.json());

// Promise.allSettled
const results = await Promise.allSettled([
  promise1,
  promise2,
  promise3
]);

results.forEach(result => {
  if (result.status === "fulfilled") {
    console.log(result.value);
  } else {
    console.error(result.reason);
  }
});

// Promise.any (first to fulfill)
const first = await Promise.any([
  fetch("/api/endpoint1"),
  fetch("/api/endpoint2"),
  fetch("/api/endpoint3")
]);

// String methods
"hello".replaceAll("l", "L");  // "heLLo"
"  trim  ".trimStart();        // "trim  "
"  trim  ".trimEnd();          // "  trim"

// Array methods
const arr = [1, 2, 3, 4, 5];
arr.at(-1);  // 5 (last element)
arr.at(-2);  // 4 (second to last)

// Object.hasOwn (safer than hasOwnProperty)
Object.hasOwn(obj, "property");

// Temporal API (Stage 3 proposal)
// const now = Temporal.Now.plainDateTimeISO();
// const date = Temporal.PlainDate.from("2024-01-15");

// Record and Tuple (Stage 2 proposal)
// const record = #{ x: 1, y: 2 };
// const tuple = #[1, 2, 3];

___________
Summary
Key Takeaways
JavaScript:
Master fundamentals: variables, functions, objects, arrays
Understand asynchronous programming: Promises, async/await
Learn modern ES6+ features: destructuring, spread, arrow functions
Know closures, prototypes, and the event loop
Practice error handling and debugging
TypeScript:
Use type annotations for safety and documentation
Leverage interfaces and types for better code organization
Master generics for reusable components
Utilize utility types and advanced type features
Enable strict mode for maximum type safety
Best Practices:
Write clean, readable, maintainable code
Follow SOLID principles
Test your code thoroughly
Handle errors gracefully
Use design patterns appropriately
Keep learning and staying updated
Resources for Further Learning
MDN Web Docs: Comprehensive JavaScript reference
TypeScript Handbook: Official TypeScript documentation
JavaScript.info: Modern JavaScript tutorial
You Don't Know JS: Deep dive book series
TypeScript Deep Dive: Free online book
Frontend Masters: Video courses
Exercism: Practice exercises.
