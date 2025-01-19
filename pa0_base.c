#include <stdio.h>            
#include <stdlib.h>            
// additional headers as needed

typedef struct node {
	char  name[64];       // node's name string
	char  type;
	struct node *child, *sibling, *parent;
	// other fields if needed
} NODE;


NODE *root; 
NODE *cwd;
char *cmd[] = {};  // fill with list of commands
// other global variables


int initialize() {
	root = (NODE *)malloc(sizeof(NODE));
	strcpy(root->name, "/");
	root->parent = root;
	root->sibling = 0;
	root->child = 0;
	root->type = 'D';
	cwd = root;
	// other steps as needed
	
	printf("Filesystem initialized!\n");
}

int main() {
	initialize();
	// other initialization as needed

	while(1) {
		printf("Enter command: ");
		// complete implementations
	}
}

