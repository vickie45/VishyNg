# JavaScript Masterclass Cheat Sheet

This cheat sheet is designed to take you from a beginner to an expert in JavaScript. It covers all the essential topics with explanations and code examples. Let's dive in!

## Table of Contents
1. [Basics](#basics)
2. [Variables and Data Types](#variables-and-data-types)
3. [Operators](#operators)
4. [Control Structures](#control-structures)
5. [Functions](#functions)
6. [Objects](#objects)
7. [Arrays](#arrays)
8. [ES6+ Features](#es6-features)
9. [Asynchronous JavaScript](#asynchronous-javascript)
10. [Error Handling](#error-handling)
11. [DOM Manipulation](#dom-manipulation)
12. [Events](#events)
13. [APIs and Fetch](#apis-and-fetch)
14. [Modules](#modules)
15. [Advanced Topics](#advanced-topics)

---

## Basics

### Hello World
```javascript
console.log("Hello, World!");
```

### Comments
```javascript
// Single-line comment

/*
Multi-line
comment
*/
```

---

## Variables and Data Types

### Declaring Variables
```javascript
let name = "John"; // Mutable
const age = 25;    // Immutable
var oldSchool = true; // Avoid using var
```

### Data Types
```javascript
let string = "Hello";
let number = 42;
let boolean = true;
let nullValue = null;
let undefinedValue;
let object = { key: "value" };
let array = [1, 2, 3];
let symbol = Symbol("unique");
```

---

## Operators

### Arithmetic Operators
```javascript
let sum = 10 + 5; // 15
let difference = 10 - 5; // 5
let product = 10 * 5; // 50
let quotient = 10 / 5; // 2
let remainder = 10 % 3; // 1
```

### Comparison Operators
```javascript
let isEqual = 10 == "10"; // true (loose equality)
let isStrictEqual = 10 === "10"; // false (strict equality)
let isNotEqual = 10 != "10"; // false
let isGreater = 10 > 5; // true
```

### Logical Operators
```javascript
let and = true && false; // false
let or = true || false; // true
let not = !true; // false
```

---

## Control Structures

### If-Else
```javascript
if (age > 18) {
    console.log("Adult");
} else {
    console.log("Minor");
}
```

### Switch
```javascript
switch (day) {
    case "Monday":
        console.log("Start of the week");
        break;
    case "Friday":
        console.log("End of the week");
        break;
    default:
        console.log("Midweek");
}
```

### Loops
```javascript
// For loop
for (let i = 0; i < 5; i++) {
    console.log(i);
}

// While loop
let i = 0;
while (i < 5) {
    console.log(i);
    i++;
}

// Do-While loop
let j = 0;
do {
    console.log(j);
    j++;
} while (j < 5);
```

---

## Functions

### Function Declaration
```javascript
function greet(name) {
    return `Hello, ${name}!`;
}
console.log(greet("John"));
```

### Function Expression
```javascript
const greet = function(name) {
    return `Hello, ${name}!`;
};
console.log(greet("John"));
```

### Arrow Functions
```javascript
const greet = (name) => `Hello, ${name}!`;
console.log(greet("John"));
```

---

## Objects

### Object Literal
```javascript
const person = {
    name: "John",
    age: 25,
    greet: function() {
        console.log(`Hello, my name is ${this.name}`);
    }
};
person.greet();
```

### Constructor Function
```javascript
function Person(name, age) {
    this.name = name;
    this.age = age;
    this.greet = function() {
        console.log(`Hello, my name is ${this.name}`);
    };
}
const john = new Person("John", 25);
john.greet();
```

---

## Arrays

### Array Methods
```javascript
let fruits = ["Apple", "Banana"];

// Add to end
fruits.push("Orange");

// Remove from end
fruits.pop();

// Add to beginning
fruits.unshift("Mango");

// Remove from beginning
fruits.shift();

// Find index
let index = fruits.indexOf("Banana");

// Remove by index
fruits.splice(index, 1);

// Iterate
fruits.forEach(fruit => console.log(fruit));
```

---

## ES6+ Features

### Let and Const
```javascript
let x = 10;
const y = 20;
```

### Template Literals
```javascript
let name = "John";
console.log(`Hello, ${name}!`);
```

### Destructuring
```javascript
const person = { name: "John", age: 25 };
const { name, age } = person;
console.log(name, age);
```

### Spread and Rest Operators
```javascript
// Spread
let arr1 = [1, 2, 3];
let arr2 = [...arr1, 4, 5];

// Rest
function sum(...numbers) {
    return numbers.reduce((acc, num) => acc + num, 0);
}
console.log(sum(1, 2, 3));
```

### Classes
```javascript
class Person {
    constructor(name, age) {
        this.name = name;
        this.age = age;
    }

    greet() {
        console.log(`Hello, my name is ${this.name}`);
    }
}

const john = new Person("John", 25);
john.greet();
```

---

## Asynchronous JavaScript

### Callbacks
```javascript
function fetchData(callback) {
    setTimeout(() => {
        callback("Data fetched");
    }, 1000);
}

fetchData(data => console.log(data));
```

### Promises
```javascript
function fetchData() {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            resolve("Data fetched");
        }, 1000);
    });
}

fetchData().then(data => console.log(data));
```

### Async/Await
```javascript
async function fetchData() {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            resolve("Data fetched");
        }, 1000);
    });
}

(async () => {
    const data = await fetchData();
    console.log(data);
})();
```

---

## Error Handling

### Try-Catch
```javascript
try {
    throw new Error("Something went wrong");
} catch (error) {
    console.error(error.message);
}
```

### Finally
```javascript
try {
    console.log("Try");
} catch (error) {
    console.error(error);
} finally {
    console.log("Finally");
}
```

---

## DOM Manipulation

### Selecting Elements
```javascript
const element = document.getElementById("myId");
const elements = document.querySelectorAll(".myClass");
```

### Modifying Elements
```javascript
element.textContent = "New Text";
element.innerHTML = "<strong>Bold Text</strong>";
element.style.color = "red";
```

### Event Listeners
```javascript
element.addEventListener("click", () => {
    console.log("Element clicked");
});
```

---

## Events

### Event Bubbling and Capturing
```javascript
element.addEventListener("click", () => {
    console.log("Element clicked");
}, true); // true for capturing, false for bubbling
```

### Event Delegation
```javascript
document.body.addEventListener("click", (event) => {
    if (event.target.matches("button")) {
        console.log("Button clicked");
    }
});
```

---

## APIs and Fetch

### Fetch API
```javascript
fetch("https://api.example.com/data")
    .then(response => response.json())
    .then(data => console.log(data))
    .catch(error => console.error(error));
```

### Async/Await with Fetch
```javascript
async function fetchData() {
    try {
        const response = await fetch("https://api.example.com/data");
        const data = await response.json();
        console.log(data);
    } catch (error) {
        console.error(error);
    }
}

fetchData();
```

---

## Modules

### Exporting
```javascript
// module.js
export const name = "John";
export function greet() {
    console.log("Hello!");
}
```

### Importing
```javascript
// main.js
import { name, greet } from './module.js';
console.log(name);
greet();
```

---

## Advanced Topics

### Closures
```javascript
function outer() {
    let count = 0;
    return function inner() {
        count++;
        console.log(count);
    };
}

const counter = outer();
counter(); // 1
counter(); // 2
```

### Prototypes
```javascript
function Person(name) {
    this.name = name;
}

Person.prototype.greet = function() {
    console.log(`Hello, my name is ${this.name}`);
};

const john = new Person("John");
john.greet();
```

### Promises and Async Patterns
```javascript
Promise.all([fetchData1(), fetchData2()])
    .then(([data1, data2]) => {
        console.log(data1, data2);
    })
    .catch(error => console.error(error));
```

### Generators
```javascript
function* idGenerator() {
    let id = 1;
    while (true) {
        yield id++;
    }
}

const gen = idGenerator();
console.log(gen.next().value); // 1
console.log(gen.next().value); // 2
```

---

This cheat sheet covers a wide range of JavaScript topics, from the basics to advanced concepts. Practice each topic thoroughly, and you'll be well on your way to becoming a JavaScript expert!