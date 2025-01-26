#include "Source.h"

// Global variables
NODE* root;
NODE* cwd;
const char* cmd[] = { "mkdir", "rmdir", "cd", "ls", "pwd", "creat", "rm", "save", "reload", "quit", NULL };
void initialize() {
    root = (NODE*)malloc(sizeof(NODE));
    strcpy(root->name, "/");
    root->parent = root;
    root->sibling = NULL;
    root->child = NULL;
    root->type = 'D';
    cwd = root;
    printf("Filesystem initialized!\n");
}
int find_command(char* user_command) {
    for (int i = 0; cmd[i] != NULL; i++) {
        if (strcmp(user_command, cmd[i]) == 0) {
            return i;
        }
    }
    return -1; 
}
void tokenize_path(char* pathname, char components[][64], int* count) {
	if (!pathname || pathname[0] == '\0') {
		*count = 0;
		return;
	}
	char* token = strtok(pathname, "/");
	*count = 0;
	while (token) {
		strcpy(components[(*count)++], token);
		token = strtok(NULL, "/");
	}
}
NODE* find_node(char* pathname) {
	if (strcmp(pathname, "/") == 0) return root;
	if (strcmp(pathname, ".") == 0 || pathname[0] == '\0') return cwd;

	char components[64][64];
	int count = 0;
	tokenize_path(pathname, components, &count);

	NODE* current = (pathname[0] == '/') ? root : cwd;
	for (int i = 0; i < count; i++) {
		NODE* child = current->child;
		while (child && strcmp(child->name, components[i]) != 0) {
			child = child->sibling;
		}
		if (!child) return NULL; 
		current = child;
	}
	return current;
}
void mkdir(char* pathname) {
	if (strcmp(pathname, "/") == 0) {
		printf("Error: Cannot create root directory again!\n");
		return;
	}
    char dirname[64], basename[64];
    strcpy(dirname, pathname);
    strcpy(basename, pathname);

    char* last_slash = strrchr(dirname, '/');
    if (last_slash) {
        *last_slash = '\0';
        strcpy(basename, last_slash + 1);
    }
    else {
        strcpy(dirname, ".");
    }

    NODE* parent = find_node(dirname);
    if (!parent || parent->type != 'D') {
        printf("Error: Invalid parent directory!\n");
        return;
    }

    NODE* child = parent->child;
    while (child) {
        if (strcmp(child->name, basename) == 0) {
            printf("Error: Directory already exists!\n");
            return;
        }
        child = child->sibling;
    }

    NODE* new_node = (NODE*)malloc(sizeof(NODE));
    strcpy(new_node->name, basename);
    new_node->type = 'D';
    new_node->child = NULL;
    new_node->sibling = parent->child;
    new_node->parent = parent;
    parent->child = new_node;
}
void pwd_helper(NODE* node) {
    if (node == root) {
        printf("/");
        return;
    }
    pwd_helper(node->parent);
    printf("%s/", node->name);
}
void pwd() {
    pwd_helper(cwd);
    printf("\n");
}
void rmdir(char* pathname) {
    NODE* target = find_node(pathname);
    if (!target || target->type != 'D') {
		printf("Error, Directory not found\n");
        return;
    }
    if (target->child) {
        printf("can not remove directory\n");
        return;
    }
	NODE* parent = target->parent;
	if (parent->child == target) {
		parent->child = target->sibling;
	}
	else {
		NODE* sibling = parent->child;
		while (sibling->sibling != target) {
			sibling = sibling->sibling;
		}
		sibling->sibling = target->sibling;
	}
	free(target);
}
void ls(char* pathname) {
	NODE* node = pathname ? find_node(pathname) : cwd; 

	if (!node) {
		printf("Error: Unable to resolve directory!");
		return;
	}
	if (node->type != 'D') {
		printf("Error: Not a directory!\n");
		return;
	}
	NODE* child = node->child;
	if (!child) {
		return;
	}
	while (child) {
		printf("%c %s\n", child->type, child->name);
		child = child->sibling;
	}
}
void cd(char* pathname) {
	NODE* target = find_node(pathname);
	if (strcmp(pathname, "..") == 0) {
		if (cwd != root) 
		{
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
		else if (target == cwd) {
			printf("Error: Already in directory!\n");
			return;
		}
		cwd = target;
		
	}
void creat(char* pathname) {
	char dirname[64], basename[64];
	strcpy(dirname, pathname);
	strcpy(basename, pathname);
	char* last_slash = strrchr(dirname, '/');
	if (last_slash) {
		*last_slash = '\0';
		strcpy(basename, last_slash + 1);
	}
	else {
		strcpy(dirname, ".");
	}
	NODE* parent = find_node(dirname);
	if (!parent || parent->type != 'D') {
		printf("Error: Invalid parent directory!\n");
		return;
	}
	NODE* child = parent->child;
	while (child) {
		if (strcmp(child->name, basename) == 0) {
			printf("Error: File already exists!\n");
			return;
		}
		child = child->sibling;
	}
	NODE* new_node = (NODE*)malloc(sizeof(NODE));
	strcpy(new_node->name, basename);
	new_node->type = 'F';
	new_node->child = NULL;
	new_node->sibling = parent->child;
	new_node->parent = parent;
	parent->child = new_node;
	printf("File created: %s\n", pathname);
}
void rm(char* pathname) {
	NODE* target = find_node(pathname);
	if (!target || target->type != 'F') {
		printf("Error: File not found!\n");
		return;
	}
	NODE* parent = target->parent;
	if (parent->child == target) {
		parent->child = target->sibling;
	}
	else {
		NODE* sibling = parent->child;
		while (sibling->sibling != target) {
			sibling = sibling->sibling;
		}
		sibling->sibling = target->sibling;
	}
	free(target);
}
void save(char* filename) {
	FILE* fp = fopen(filename, "w");
	if (!fp) {
		printf("Error: Could not open file %s for writing.\n", filename);
		return;
	}
	save_helper(fp, root->child, "/");
	fclose(fp);
}
void save_helper(FILE* fp, NODE* node, char* path) {
	if (!node) return;
	char full_path[128];
	if (strcmp(path, "/") == 0) { 
		sprintf(full_path, "/%s", node->name);
	}
	else {
		sprintf(full_path, "%s/%s", path, node->name);
	}
	fprintf(fp, "%c %s\n", node->type, full_path);
	save_helper(fp, node->child, full_path);
	save_helper(fp, node->sibling, path);
}
void save_node(NODE* node, FILE* file) {
	if (!node) return;
	fprintf(file, "%s\n", node->name);
	fprintf(file, "%c\n", node->type);
	if (node->child) {
		fprintf(file, "child\n");
		save_node(node->child, file);
	}
	if (node->sibling) {
		fprintf(file, "sibling\n");
		save_node(node->sibling, file);
	}
	if (!node->child && !node->sibling) {
		fprintf(file, "end\n");
	}
}
void reload(char* filename) {
	FILE* file = fopen(filename, "r");
	if (!file) {
		printf("Error: Unable to open file!\n");
		return;
	}
	char buffer[64];
	fgets(buffer, 64, file);
	if (strcmp(buffer, "root\n") != 0) {
		printf("Error: Invalid file format!\n");
		fclose(file);
		return;
	}
	fgets(buffer, 64, file);
	buffer[strlen(buffer) - 1] = '\0';
	strcpy(root->name, buffer);
	fgets(buffer, 64, file);
	root->type = buffer[0];
	fgets(buffer, 64, file);
	if (strcmp(buffer, "cwd\n") != 0)
	{
		printf("Error: Invalid file format!\n");
		fclose(file);
		return;
	}
	fgets(buffer, 64, file);
	buffer[strlen(buffer) - 1] = '\0';
	cwd = find_node(buffer);
	if (!cwd) {
		printf("Error: Invalid file format!\n");
		fclose(file);
		return;
	}
	fgets(buffer, 64, file);
	if (strcmp(buffer, "tree\n") != 0) {
		printf("Error: Invalid file format!\n");
		fclose(file);
		return;
	}
	NODE* node = root;
	while (1) {
		fgets(buffer, 64, file);
		buffer[strlen(buffer) - 1] = '\0';
		strcpy(node->name, buffer);
		fgets(buffer, 64, file);
		node->type = buffer[0];
		fgets(buffer, 64, file);
		if (strcmp(buffer, "child\n") == 0) {
			NODE* child = (NODE*)malloc(sizeof(NODE));
			child->parent = node;
			node->child = child;
			node = child;
		}
		else if (strcmp(buffer, "sibling\n") == 0) {
			NODE* sibling = (NODE*)malloc(sizeof(NODE));
			sibling->parent = node->parent;
			node->sibling = sibling;
			node = sibling;
		}
		else if (strcmp(buffer, "parent\n") == 0) {
			node = node->parent;
		}
		else {
			break;
		}
	}
	fclose(file);
}	
void quit() {
	save("fssim_Gudino.txt");
	exit(0);
}