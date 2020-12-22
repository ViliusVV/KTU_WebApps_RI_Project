<template>
  <v-data-table
    :headers="headers"
    :items="users"
    class="elevation-1 content-margin"
  >
    <template v-slot:top>
      <v-toolbar flat>
        <!-- TABLE TITLE -->
        <v-toolbar-title>Users</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <v-dialog v-model="dialog" max-width="500px">
          <template v-slot:activator="{ on, attrs }">
            <!-- NEW ITEM BUTTON -->
            <v-btn color="primary" dark class="mb-2" v-bind="attrs" v-on="on">
              New User
            </v-btn>
          </template>
          <v-card>
            <v-card-title>
              <span class="headline">{{ formTitle }}</span>
            </v-card-title>

            <v-card-text>
              <v-container>
                <v-row>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.name"
                      label="Name"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.surname"
                      label="Surname"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.username"
                      label="Username"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.passwordHash"
                      label="Password"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-select
                      :items="roles"
                      v-model="editedItem.role"
                      label="Role"
                    ></v-select>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.contactInfo.email"
                      label="Email"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.contactInfo.phoneNumber"
                      label="Phone"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.contactInfo.city"
                      label="City"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.contactInfo.address"
                      label="Address"
                    ></v-text-field>
                  </v-col>
                </v-row>
              </v-container>
            </v-card-text>

            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" text @click="close"> Cancel </v-btn>
              <v-btn color="blue darken-1" text @click="save"> Save </v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <v-dialog v-model="dialogDelete" max-width="500px">
          <v-card>
            <v-card-title class="headline"
              >Are you sure you want to delete this item?</v-card-title
            >
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" text @click="closeDelete"
                >Cancel</v-btn
              >
              <v-btn color="blue darken-1" text @click="deleteItemConfirm"
                >OK</v-btn
              >
              <v-spacer></v-spacer>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-toolbar>
    </template>
    <template v-slot:[`item.actions`]="{ item }">
      <v-icon small class="mr-2" @click="editItem(item)"> mdi-pencil </v-icon>
      <v-icon small @click="deleteItem(item)"> mdi-delete </v-icon>
    </template>
    <template v-slot:no-data>
      <v-btn color="primary" @click="initialize"> Reset </v-btn>
    </template>
  </v-data-table>
</template>

<script>
import { userService } from '../services/userService';
import { Role } from '../helpers/role'

export default {
  data: () => ({
    dialog: false,
    dialogDelete: false,
    // HEADER STRINGS
    headers: [
      {
        text: "Name",
        align: "start",
        value: "name",
      },
      { text: "Surname", value: "surname" },
      { text: "Username", value: "username" },
      { text: "Role", value: "role" },
      { text: "Email", value: "contactInfo.email" },
      { text: "Phone", value: "contactInfo.phoneNumber" },
      { text: "City", value: "contactInfo.city" },
      { text: "Address", value: "contactInfo.address"},
      { text: 'Actions', value: 'actions', sortable: false },
    ],
    // MAIN STORAGE LIST
    users: [],
    editedIndex: -1,
    // EDIT MODEL
    editedItem: {
        name: '',
        surname: '',
        username: '',
        role: Role.Referee,
        passwordHash: '',
        contactInfo: {
            email: '',
            phoneNumber: '',
            city: '',
            address: ''
        },
        id: '',
        createdAt: ''
    },
    // DEFAULT ITEM
    defaultItem: {
        name: '',
        surname: '',
        username: '',
        role: Role.Referee,
        passwordHash: '',
        contactInfo: {
            email: '',
            phoneNumber: '',
            city: '',
            address: ''
        },
        id: '',
        createdAt: ''
    },
    roles: [Role.Admin, Role.Referee]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New Item" : "Edit Item";
    },
  },

  watch: {
    dialog(val) {
      val || this.close();
    },
    dialogDelete(val) {
      val || this.closeDelete();
    },
  },

  created() {
    this.initialize();
  },

  methods: {
    initialize() {
      userService.getAll().then(data => this.users = data);
    },

    editItem(item) {
      this.editedIndex = this.users.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.editedItem.passwordHash = '';
      this.dialog = true;
    },

    deleteItem(item) {
      this.editedIndex = this.users.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialogDelete = true;
    },

    deleteItemConfirm() {
      userService.deleteItem(this.editedItem.id)
      this.users.splice(this.editedIndex, 1);
      this.closeDelete();
    },

    close() {
      this.dialog = false;
      this.$nextTick(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      });
    },

    closeDelete() {
      this.dialogDelete = false;
      this.$nextTick(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      });
    },

    save() {
      console.log(`Saving item ${JSON.stringify(this.editedItem)}`)
      if (this.editedIndex > -1) { // If index greater than 0 when we are editing existing entry
        if(this.editedItem.passwordHash.length <= 1){
          delete this.editedItem.passwordHash;
        }
        userService.update(this.editedItem.id, this.editedItem);
        Object.assign(this.users[this.editedIndex], this.editedItem);
      } else { // We are creating new
        userService.create(this.editedItem);
        this.users.push(this.editedItem);
      }
      this.close();
    },
  },
};
</script>