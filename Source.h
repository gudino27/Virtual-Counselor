#ifndef SOURCE_H
#define SOURCE_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// Define the node structure for the filesystem tree
typedef struct node {
    char name[64];       // Node's name
    char type;           // 'D' for directory, 'F' for file
    struct node* child;  // Pointer to the first child
    struct node* sibling; // Pointer to the next sibling
    struct node* parent;  // Pointer to the parent
} NODE;

// Global variables
extern NODE* root; // Root of the filesystem tree
extern NODE* cwd;  // Current working directory
extern const char* cmd[];

// Function prototypes
void initialize();
int find_command(char* user_command);
NODE* find_node(char* pathname);
void tokenize_path(char* pathname, char components[][64], int* count);
void pwd_helper(NODE* node);
void pwd();
void ls(char* pathname);
void mkdir(char* pathname);
void rmdir(char* pathname);
void cd(char* pathname);
void creat(char* pathname);
void rm(char* pathname);
void save(char* filename);
void save_helper(FILE* fp, NODE* node, char* path);
void save_node(NODE* node, FILE* file);
void reload(char* filename);
void quit();

#endif // !SOURCE_H

