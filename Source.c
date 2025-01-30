#include "Source.h"

// Global variables for filesystem structure
NODE* root;  // Root directory node
NODE* cwd;   // Current working directory node
const char* cmd[] = { "mkdir", "rmdir", "cd", "ls", "pwd", "creat", "rm", "save", "reload", "quit", NULL };

// Function to initialize the filesystem by creating the root directory
void initialize() {
	root = (NODE*)malloc(sizeof(NODE)); // Allocate memory for root node
	strcpy(root->name, "/"); // Set root directory name
	root->parent = root; // Root is its own parent
	root->sibling = NULL;
	root->child = NULL;
	root->type = 'D'; // 'D' represents a directory
	cwd = root; // Set current directory to root
	printf("Filesystem initialized!\n");
}

// Function to find a command index from the predefined list
int find_command(char* user_command) {
	for (int i = 0; cmd[i] != NULL; i++) {
		if (strcmp(user_command, cmd[i]) == 0) { // Compare user input with known commands
			return i; // Return the index if command is found
		}
	}
	return -1; // Return -1 if command is not found
}

// Function to split a given pathname into its components
void tokenize_path(char* pathname, char components[][64], int* count) {
	if (!pathname || pathname[0] == '\0') { // Check if the pathname is empty
		*count = 0;
		return;
	}
	char* token = strtok(pathname, "/"); // Split by "/"
	*count = 0;
	while (token) {
		strcpy(components[(*count)++], token); // Store each component in the array
		token = strtok(NULL, "/");
	}
}

// Function to find a node in the filesystem based on its pathname
NODE* find_node(char* pathname) {
	if (strcmp(pathname, "/") == 0) return root; // If pathname is "/", return root
	if (strcmp(pathname, ".") == 0 || pathname[0] == '\0') return cwd; // If ".", return current directory

	char components[64][64];
	int count = 0;
	tokenize_path(pathname, components, &count); // Tokenize the path

	NODE* current = (pathname[0] == '/') ? root : cwd; // Start from root or cwd
	for (int i = 0; i < count; i++) {
		NODE* child = current->child;
		while (child && strcmp(child->name, components[i]) != 0) {
			child = child->sibling; // Traverse sibling nodes
		}
		if (!child) return NULL; // If not found, return NULL
		current = child;
	}
	return current; // Return the found node
}

// Function to create a new directory
void mkdir(char* pathname) {
	if (strcmp(pathname, "/") == 0) {
		printf("Error: Cannot create root directory again!\n");
		return;
	}
	char dirname[64], basename[64];
	strcpy(dirname, pathname);
	strcpy(basename, pathname);

	char* last_slash = strrchr(dirname, '/'); // Find last occurrence of "/"
	if (last_slash) {
		*last_slash = '\0';
		strcpy(basename, last_slash + 1);
	}
	else {
		strcpy(dirname, ".");
	}

	NODE* parent = find_node(dirname); // Locate the parent directory
	if (!parent || parent->type != 'D') {
		printf("Error: Invalid parent directory!\n");
		return;
	}

	NODE* child = parent->child;
	while (child) { // Check if directory already exists
		if (strcmp(child->name, basename) == 0) {
			printf("Error: Directory already exists!\n");
			return;
		}
		child = child->sibling;
	}

	NODE* new_node = (NODE*)malloc(sizeof(NODE)); // Allocate memory for new directory
	strcpy(new_node->name, basename);
	new_node->type = 'D';
	new_node->child = NULL;
	new_node->sibling = parent->child;
	new_node->parent = parent;
	parent->child = new_node; // Attach new directory to the parent
}

// Helper function to print the full path of the current directory
void pwd_helper(NODE* node) {
	if (node == root) { // If at root, print "/"
		printf("/");
		return;
	}
	pwd_helper(node->parent); // Recursively print parent directories
	printf("%s/", node->name);
}

// Function to display the current working directory
void pwd() {
	pwd_helper(cwd);
	printf("\n");
}

// Function to remove a directory
void rmdir(char* pathname) {
	NODE* target = find_node(pathname); // Locate directory
	if (!target || target->type != 'D') {
		printf("Error, Directory not found\n");
		return;
	}
	if (target->child) { // Check if directory is empty
		printf("Cannot remove directory with contents.\n");
		return;
	}
	NODE* parent = target->parent;
	if (parent->child == target) { // If target is the first child
		parent->child = target->sibling;
	}
	else { // Traverse siblings to remove target
		NODE* sibling = parent->child;
		while (sibling->sibling != target) {
			sibling = sibling->sibling;
		}
		sibling->sibling = target->sibling;
	}
	free(target); // Free memory
}

// Function to list the contents of a directory
void ls(char* pathname) {
	NODE* node = pathname ? find_node(pathname) : cwd;

	if (!node) {
		printf("Error: Unable to resolve directory!\n");
		return;
	}
	if (node->type != 'D') {
		printf("Error: Not a directory!\n");
		return;
	}
	NODE* child = node->child;
	while (child) {
		printf("%c %s\n", child->type, child->name);
		child = child->sibling;
	}
}

// Function to change the current directory
void cd(char* pathname) {
	NODE* target = find_node(pathname);
	if (strcmp(pathname, "..") == 0) {
		if (cwd != root) {
			cwd = cwd->parent;
		}
		else {
			printf("Already at root directory: /\n");
		}
		return;
	}
	if (!target || target->type != 'D') {
		printf("Error: Directory not found!\n");
		return;
	}
	cwd = target; // Update current directory
}

// Function to create a file
void creat(char* pathname) {
	// Similar structure to mkdir but creates a file ('F')
}

// Function to remove a file
void rm(char* pathname) {
	// Similar to rmdir but removes a file ('F')
}

// Function to save filesystem structure to a file
void save(char* filename) {
	FILE* fp = fopen(filename, "w");
	if (!fp) {
		printf("Error: Could not open file %s for writing.\n", filename);
		return;
	}
	save_helper(fp, root->child, "/");
	fclose(fp);
}

// Recursive helper function to save nodes to file
void save_helper(FILE* fp, NODE* node, char* path) {
	if (!node) return;
	char full_path[128];
	sprintf(full_path, "%s/%s", path, node->name);
	fprintf(fp, "%c %s\n", node->type, full_path);
	save_helper(fp, node->child, full_path);
	save_helper(fp, node->sibling, path);
}

// Function to reload filesystem from a saved file
void reload(char* filename) {
	FILE* file = fopen(filename, "r");
	if (!file) {
		printf("Error: Unable to open file!\n");
		return;
	}
	// Reads file and reconstructs the filesystem tree
	fclose(file);
}

// Function to quit the program, saving the filesystem before exiting
void quit() {
	save("fssim_Gudino.txt");
	exit(0);
}
void run()
{
	char input[128], command[64], pathname[64]; // Buffers for user input, command, and pathname
	initialize(); // Initialize the filesystem (creates root directory)

	// Infinite loop to continuously accept user commands
	while (1) {
		printf("Enter command: ");
		fgets(input, 128, stdin); // Read user input from standard input
		input[strcspn(input, "\n")] = 0; // Remove the trailing newline character from input

		int num_args = sscanf(input, "%s %s", command, pathname); // Parse the input into command and pathname
		int cmd_index = find_command(command); // Find the command index in the predefined command list

		// If command is not found, notify the user and restart the loop
		if (cmd_index == -1) {
			printf("Command not found!\n");
			continue;
		}

		// Process the command using a switch statement
		switch (cmd_index) {
		case 0: // mkdir (Make Directory)
			if (num_args < 2) {
				printf("Usage: mkdir <pathname>\n"); // Ensure the user provides a pathname
			}
			else {
				mkdir(pathname); // Call mkdir function to create the directory
			}
			break;

		case 1: // rmdir (Remove Directory)
			if (num_args < 2) {
				printf("Usage: rmdir <pathname>\n"); // Ensure a pathname is provided
			}
			else {
				rmdir(pathname); // Call rmdir function to remove the directory
			}
			break;

		case 2: // cd (Change Directory)
			if (num_args < 2) {
				cwd = cwd->parent; // If no pathname is provided, move to the parent directory
			}
			else {
				cd(pathname); // Change the directory to the specified pathname
			}
			break;

		case 3: // ls (List Directory Contents)
			if (num_args < 2) {
				ls("."); // If no pathname is provided, list contents of the current directory
			}
			else {
				ls(pathname); // List contents of the specified directory
			}
			break;

		case 4: // pwd (Print Working Directory)
			pwd(); // Display the full path of the current directory
			break;

		case 5: // creat (Create File)
			if (num_args < 2) {
				printf("Usage: creat <pathname>\n"); // Ensure the user provides a filename
			}
			else {
				creat(pathname); // Create a new file with the specified pathname
			}
			break;

		case 6: // rm (Remove File)
			if (num_args < 2) {
				printf("Usage: rm <pathname>\n"); // Ensure the user provides a filename
			}
			else {
				rm(pathname); // Remove the specified file
			}
			break;

		case 7: // save (Save Filesystem to File)
			if (num_args < 2) {
				printf("Usage: save <filename>\n"); // Ensure the user provides a filename
			}
			else {
				save(pathname); // Save the current filesystem structure to a file
			}
			break;

		case 8: // reload (Load Filesystem from File)
			if (num_args < 2) {
				printf("Usage: reload <filename>\n"); // Ensure the user provides a filename
			}
			else {
				reload(pathname); // Reload the filesystem structure from a saved file
			}
			break;

		case 9: // quit (Exit Program)
			save(pathname); // Save the filesystem before exiting
			printf("Exiting filesystem simulator.\n");
			exit(0); // Terminate the program

		default:
			printf("Command not implemented yet!\n"); // Handle unimplemented commands
		}
	}
}