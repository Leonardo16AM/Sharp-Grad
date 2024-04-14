# Sharp Grad - A C# Automatic Differentiation Library

Sharp Grad is a lightweight, flexible, and efficient automatic differentiation (AD) library implemented in C#. It supports the construction and differentiation of complex mathematical expressions with ease, providing a robust foundation for implementing machine learning models and scientific computations.

## Features:
- Supports basic arithmetic operations (+, -, *, /, ^).
- Provides activation functions like ReLU and Tanh.
- Implements backpropagation for efficient gradient computation.

## Code Structure:

The `value` class forms the core of Sharp Grad. Each `Value` instance represents a node in the computational graph and contains the following:

- `Data`: Stores the numerical value of the node.
- `Grad`: Stores the gradient of the function with respect to the node.
- `LeftChildren`: A `Value` objects, representing a nodes that this node depends on. It should be a left operand in computation.
- `RightChildren`: A `Value` objects, representing a nodes that this node depends on. It should be a right operand in computation
- `Backward`: Delegate for performing the backward pass during backpropagation.
- `BackwardEmpt`: A function that updates gradients of child nodes during the backward pass.

### Basic Arithmetic Operations:

Sharp Grad supports addition (+), subtraction (-), multiplication (*), division (/), and exponentiation (^). These are implemented as operator overloads, making them easy to use in code.

### Activation Functions:

Sharp Grad currently includes the following activation functions: ReLU and Tanh. These are critical for building deep learning models and can be easily applied to any `Value` object.

### Backpropagation:

Sharp Grad implements backpropagation for computing gradients efficiently. It does this by constructing a topological sort of the nodes in the computational graph and then using this ordering to perform the backward pass.

## Getting Started:

Instantiate `Value` objects for your variables and constants. You can then combine these objects using arithmetic operations and functions to construct the desired mathematical expression. Finally, call the `Backpropagate` method on the output `Value` to compute the gradients.

## Future Work:

We aim to continually expand and optimize the functionalities of Sharp Grad. Future additions may include more mathematical operations, expanded support for various data types, and additional performance optimizations.

## Contributing:

Contributions to Sharp Grad are always welcome!