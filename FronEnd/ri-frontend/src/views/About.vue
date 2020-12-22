<template>
  <v-container fill-height fluid>
    <v-row align="center"
        justify="center">
        <v-col>  
          <v-card
            class="mx-auto image-container-card"
            max-width="1000px"
          >
            <v-card-title>
              RI flyer
            </v-card-title>

            <v-img
              class="ri-flyer-image"
              :src="require('../assets/ri-card.jpg')"
            />
          </v-card>
      </v-col>
    </v-row>
  </v-container>

</template>
