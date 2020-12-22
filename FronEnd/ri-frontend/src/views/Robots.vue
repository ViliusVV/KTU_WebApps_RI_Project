<template>
  <v-data-table
    :headers="headers"
    :items="robots"
    class="elevation-1 content-margin"
  >
    <template v-slot:top>
      <v-toolbar flat>
        <!-- TABLE TITLE -->
        <v-toolbar-title>Robots</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <v-dialog v-model="dialog" max-width="500px">
          <template v-slot:activator="{ on, attrs }">
            <!-- NEW ITEM BUTTON -->
            <v-btn color="primary" dark class="mb-2" v-bind="attrs" v-on="on">
              New Robot
            </v-btn>
          </template>
          <v-card>
            <v-card-title>
              <span class="headline">{{ formTitle }}</span>
            </v-card-title>

            <v-card-text>
              <v-container>
                <v-row>
                  <v-col cols="12" sm="12" md="12">
                    <v-text-field
                      v-model="editedItem.name"
                      label="Name"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="12" md="12">
                    <v-select
                      :items="types"
                      v-model="editedItem.type"
                      label="Role"
                    ></v-select>
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
import { robotService } from '../services/robotService';

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
      { text: "Type", value: "type" },
      { text: 'Actions', value: 'actions', sortable: false },
    ],
    // MAIN STORAGE LIST
    robots: [],
    editedIndex: -1,
    // EDIT MODEL
    editedItem: {
        name: '',
        type: "Advanced",
        lapTimes: [],
        id: '',
        createdAt: ''
    },
    // DEFAULT ITEM
    defaultItem: {
        name: '',
        type: "Advanced",
        lapTimes: [],
        id: '',
        createdAt: ''
    },
    types: ["Advanced", "LEGO"],
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
      robotService.getAll().then(data => this.robots = data);
    },

    editItem(item) {
      this.editedIndex = this.robots.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialog = true;
    },

    deleteItem(item) {
      this.editedIndex = this.robots.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialogDelete = true;
    },

    deleteItemConfirm() {
      robotService.deleteItem(this.editedItem.id)
      this.robots.splice(this.editedIndex, 1);
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
        robotService.update(this.editedItem.id, this.editedItem);
        Object.assign(this.robots[this.editedIndex], this.editedItem);
      } else { // We are creating new
        robotService.create(this.editedItem);
        this.robots.push(this.editedItem);
      }
      this.close();
    },
  },
};
</script>