#ifndef SOURCE_H
#define SOURCE_H
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
// Define the node structure for the filesystem tree
typedef struct node {
    char name[64];       // Name of the directory or file
    char type;           // 'D' for directory, 'F' for file
    struct node* child;  // Pointer to the first child node (subdirectory or file)
    struct node* sibling; // Pointer to the next sibling node (next directory or file in the same level)
    struct node* parent;  // Pointer to the parent directory
} NODE;
// Global variables
extern NODE* root; // Root of the filesystem tree
extern NODE* cwd;  // Current working directory
extern const char* cmd[]; // Array of supported commands
// Function prototypes
// Initializes the filesystem by creating the root directory.
void initialize();
// Finds the index of a command in the command list based on user input.
int find_command(char* user_command);
// Finds and returns the node corresponding to the given pathname.
NODE* find_node(char* pathname);
// Splits a pathname into individual directory/file components for easier traversal.
void tokenize_path(char* pathname, char components[][64], int* count);
// Helper function for recursively printing the full path of the current directory.
void pwd_helper(NODE* node);
// Prints the absolute path of the current working directory.
void pwd();
// Lists the contents of a directory specified by pathname, or the current directory if none is provided.
void ls(char* pathname);
// Creates a new directory at the specified pathname.
void mkdir(char* pathname);
// Removes an existing directory, ensuring it is empty before deletion.
void rmdir(char* pathname);
// Changes the current working directory to the specified pathname.
void cd(char* pathname);
// Creates a new file at the specified pathname.
void creat(char* pathname);
// Removes a file from the filesystem.
void rm(char* pathname);
// Saves the current filesystem structure to a specified file.
void save(char* filename);
// Recursively writes filesystem data to a file during the save process.
void save_helper(FILE* fp, NODE* node, char* path);
// Loads a previously saved filesystem structure from a file.
void reload(char* filename);
// Saves the current filesystem state and exits the program.
void quit();
// Runs the main command processing loop, continuously accepting user input and executing commands.
void run();
#endif // !SOURCE_H
