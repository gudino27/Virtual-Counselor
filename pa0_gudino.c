#include "source.h"


// Main loop
int main() {
    char input[128], command[64], pathname[64];
    initialize();

    while (1) {
        printf("Enter command: ");
        fgets(input, 128, stdin);
        input[strcspn(input, "\n")] = 0; // Remove newline character

        int num_args = sscanf(input, "%s %s", command, pathname);
        int cmd_index = find_command(command);

        if (cmd_index == -1) {
            printf("Command not found!\n");
            continue;
        }

        switch (cmd_index) {
        case 0: // mkdir
            if (num_args < 2) {
                printf("Usage: mkdir <pathname>\n");
            }
            else {
                mkdir(pathname);
            }
            break;
		case 1: // rmdir
			if (num_args < 2) {
				printf("Usage: rmdir <pathname>\n");
			}
			else {
				rmdir(pathname);
			}
			break;
		case 2: // cd
			if (num_args < 2) {
				cwd = cwd->parent;
			}
			
			else {
				cd(pathname);
			}
			break;
		case 3: // ls
			if (num_args < 2) {
				ls(".");
			}
			else {
                    
				ls(pathname);
			}
			break;
		case 4: // pwd
			pwd();
			break;
		case 5: // creat
			if (num_args < 2) {
				printf("Usage: creat <pathname>\n");
			}
			else {    
				creat(pathname);
			}
			break;
		case 6: // rm
			if (num_args < 2) {
				printf("Usage: rm <pathname>\n");
			}
			else {
				rm(pathname);
			}
			break;
		case 7: // save
			if (num_args < 2) {
				printf("Usage: save <filename>\n");
			}
			else {
				save(pathname);
			}
			break;
		case 8: // reload
			if (num_args < 2) {
				printf("Usage: reload <filename>\n");
			}
			else {
				reload(pathname);
			}
			break;        
        case 9: // quit
			save(pathname);
            printf("Exiting filesystem simulator.\n");
            exit(0);
        default:
            printf("Command not implemented yet!\n");
        }
    }

    return 0;
}
