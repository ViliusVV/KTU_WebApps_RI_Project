<template>
  <v-data-table
    :headers="headers"
    :items="teams"
    class="elevation-1 content-margin"
  >
    <template v-slot:top>
      <v-toolbar flat>
        <!-- TABLE TITLE -->
        <v-toolbar-title>Teams</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <v-dialog v-model="dialog" max-width="500px" transition="fab-transition">
          <template v-slot:activator="{ on, attrs }">
            <!-- NEW ITEM BUTTON -->
            <v-btn color="primary" dark class="mb-2" v-bind="attrs" v-on="on">
              New Team
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
                  <v-col cols="12" sm="6" md="6">
                    <v-text-field
                      v-model="editedItem.city"
                      label="City"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="6">
                    <v-text-field
                      v-model="editedItem.represents"
                      label="Representative of"
                    ></v-text-field>
                  </v-col>
                <v-col cols="12" sm="12" md="12">
                    <v-select
                      :items="participants"
                      item-value="id"
                      item-text="name"
                      v-model="editedItem.teamMembers"
                      label="Team members"
                      attach
                      chips
                      multiple
                    ></v-select>
                  </v-col>
                <v-col cols="12" sm="12" md="12">
                    <v-select
                      :items="robots"
                      item-value="id"
                      item-text="name"
                      v-model="editedItem.robots"
                      label="Robots"
                      attach
                      chips
                      multiple
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
    <template v-slot:[`item.members`]="{ item }">
        <div v-for="member in item.teamMembers" v-bind:key="member">
        <v-chip
            class="ma-2"
            color="yellow"
            
            >
            <v-icon left>
                mdi-account-circle-outline
            </v-icon>
            <div>{{getUser(member)}}</div>
        </v-chip>
        </div>
    </template>
    <template v-slot:[`item.robots`]="{ item }">
        <div v-for="robot in item.robots" v-bind:key="robot">
        <v-chip
            class="ma-2"
            color="red"
            >
            <v-icon left>
                mdi-robot
            </v-icon>
            <div>{{getRobot(robot)}}</div>
        </v-chip>
        </div>
    </template>
    <template v-slot:no-data>
      <v-btn color="primary" @click="initialize"> Reset </v-btn>
    </template>
  </v-data-table>
</template>

<script>
import { teamService } from '../services/teamService';
import { robotService } from '../services/robotService';
import { participantService } from '../services/participantService';

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
      { text: "Name", value: "name" },
      { text: "City", value: "city" },
      { text: "Representative of", value: "represents" },
      { text: "Members", value: "members" },
      { text: "Robots", value: "robots" },
      { text: 'Actions', value: 'actions', sortable: false },
    ],
    // MAIN STORAGE LIST
    teams: [],
    robots: [],
    participants: [],
    editedIndex: -1,
    // EDIT MODEL
    editedItem: {
        name: '',
        city: '',
        represents: '',
        teamMembers: [ ],
        robots: [],
        id: '',
        createdAt: ''
    },
    // DEFAULT ITEM
    defaultItem: {
        name: '',
        city: '',
        represents: '',
        teamMembers: [ ],
        robots: [],
        id: '',
        createdAt: ''
    },
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
      teamService.getAll().then(data => this.teams = data);
      participantService.getAll().then(data => this.participants = data);
      robotService.getAll().then(data => this.robots = data);
    },

    editItem(item) {
      this.editedIndex = this.teams.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialog = true;
    },

    deleteItem(item) {
      this.editedIndex = this.teams.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialogDelete = true;
    },

    deleteItemConfirm() {
      teamService.deleteItem(this.editedItem.id)
      this.teams.splice(this.editedIndex, 1);
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
        teamService.update(this.editedItem.id, this.editedItem);
        Object.assign(this.teams[this.editedIndex], this.editedItem);
      } else { // We are creating new
        teamService.create(this.editedItem);
        this.teams.push(this.editedItem);
      }
      this.close();
    },

    getRobot(id){
        let rob = ""
        this.robots.forEach(robot => {
            if(robot.id === id){
                rob  = robot.name;
            }
        });

        return rob;
    },
    getUser(id){
    let part = ""
    this.participants.forEach(participant => {
        if(participant.id === id){
            part  = `${participant.name} ${participant.surname}`;
        }
    });

    return part;
    }
  },
};
</script>