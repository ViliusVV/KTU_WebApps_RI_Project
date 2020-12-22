<template>
  <v-data-table
    :headers="headers"
    :items="times"
    class="elevation-1 content-margin"
  >
    <template v-slot:top>
      <v-toolbar flat>
        <!-- TABLE TITLE -->
        <v-toolbar-title>LEGO LineFollower Scoreboard</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-spacer></v-spacer>
      </v-toolbar>
    </template>
    <template v-slot:[`item.place`]="{ item }">
      <v-chip
          class="ma-2"
          :color="getColor(item)"
          v-show="item.place <= 3"
          >
          <v-icon left>
              mdi-counter
          </v-icon>
          <div>{{item.place}}</div>
      </v-chip>
      <div v-show="item.place > 3">{{item.place}}</div>
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
    // HEADER STRINGS
    headers: [
      {
        text: "Place",
        align: "start",
        value: "place",
      },
      { text: "Robot name", value: "robotName" },
      { text: "Round 1 time (ms)", value: "round1" },
      { text: "Round 2 time (ms)", value: "round2" },
      { text: "Round 3 time (ms)", value: "round3" },
    ],
    robots: [],
    times: [],
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New Item" : "Edit Item";
    },
  },

  watch: {
  },

  created() {
    this.initialize();
  },

  methods: {
    initialize() {
      robotService.getAll().then(data => {
        this.robots = data;

        console.log("Robots");
        console.log(JSON.stringify(this.robots));
        this.robots.forEach(robot => {
          let entry = {
            place: null,
            robotName: '',
            round1: null,
            round2: null,
            round3: null
          };

          entry.robotName = robot.name;
          if(robot.lapTimes[0] !== undefined){
            entry.round1 = robot.lapTimes[0].timeElapsedMs
          }
          if(robot.lapTimes[1] !== undefined){
            entry.round2 = robot.lapTimes[1].timeElapsedMs
          }
          if(robot.lapTimes[2] !== undefined){
            entry.round3 = robot.lapTimes[2].timeElapsedMs
          }
          if(robot.type == "LEGO"){
            this.times.push(entry);
            this.sortTimeEntries(this.times);
          }
        });
      });
    },
    getColor(item){
      if(item.place === 1){
        return "yellow"
      }

      if(item.place === 2){
        return "silver"
      }

      if(item.place === 3){
        return "#cd7f32"
      }
    },
    sortTimeEntries(entries){
      entries.sort(this.compareTimeEntry)

      let place = 1;
      entries.forEach(entry => {
        entry.place = place;
        place = place + 1;
      })
      
      console.log(JSON.stringify(entries))
    },

    compareTimeEntry(x, y){
      let xMin = this.getMin(x);
      let yMin = this.getMin(y);
      console.log(`${xMin} ${yMin}`);
      return xMin > yMin ? 1 : -1;
    },

    getMin(times){
      let minVal = Number.MAX_SAFE_INTEGER;
      if(times.round1){
        if(minVal > +times.round1){
          minVal = times.round1
        }
      }

      if(times.round2){
        if(minVal > +times.round2){
          minVal = times.round2
        }
      }
      if(times.round3){
        if(minVal > +times.round3){
          minVal = times.round3
        }
      }
      return minVal;
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