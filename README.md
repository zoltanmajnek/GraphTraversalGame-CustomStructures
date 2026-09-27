# Graph Traversal vs. AI Game 
This repository contains the original university project code from 2024. I am currently actively refactoring the project to apply modern clean code principles, which includes translating Hungarian variable/class names and comments into English.

## Overview
A graph traversal game built in C# where the player competes against a computer opponent. The player's objective is to navigate the graph and reach the central "computer" (target node). Meanwhile, the AI dynamically cuts edges between nodes to block the player's path and prevent them from winning.

## Technical Highlights
This project was developed to demonstrate a deep understanding of core computer science concepts. Instead of using built-in C# collections, I implemented the underlying data structures and algorithms from scratch using reference/pointer-based architecture:

### Custom Data Structures
* **Linked List:** Custom implementation for sequential data handling.
* **Stack:** Custom LIFO structure used for pathfinding algorithms.
* **Graph:** Pointer-based node and edge representation.

### AI Opponent Strategies
The computer opponent evaluates the graph state and cuts edges based on three selectable algorithms:
* **Random:** Severs a random available connection.
* **Greedy (Mohó):** Makes the locally optimal choice at that specific stage.
* **Backtracking:** Explores all possible paths to recursively find the most effective edge to cut.

## Tech Stack
* **Language:** C#
* **Concepts:** Data Structures, Object-Oriented Programming (OOP), Algorithmic Pathfinding, Memory/Reference Management