import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AvManifestService } from '../../service/UtubeService';

export interface AvManifest {
  id: string;
  title: string;
  url: string; // This will be calculated in your component/service
  uploadDate: Date;
  description: string;
  keywords: string[];
  audioStreams: AudioStream[];
  videoStreams: VideoStream[];
}

// video-stream.ts
export interface VideoStream {
  url: string;
  container: string;
  size: string;
  bitrate: string;
  videoCodec: string;
  videoQuality: string;
  videoResolution: string;
}

// audio-stream.ts
export interface AudioStream {
  url: string;
  container: string;
  size: string;
  bitrate: string;
  audioCodec: string;
  audioLanguage?: string; // Optional
  isAudioLanguageDefault?: boolean; // Optional
}

// interface AvManifest {
//   id: string;
//   title: string;
//   url: string; // Generated dynamically
//   uploadDate: Date; // Assuming you receive it in Date format
//   description: string;
//   //duration?: TimeSpan; // Optional property
//   keywords: string[];
// }


@Component({
  selector: 'file-quality-picker',
  standalone: false,
  templateUrl: './file-quality-picker.component.html',
  styleUrl: './file-quality-picker.component.css'
})
export class FileQualityPickerComponent {

  resourceId: string = '';
  videoStreams: VideoStream[] = [];
  audioStreams: AudioStream[] = [];
  avManifest? : AvManifest;

  selectedVStreams: string[] = [];
  selectedAStreams: string[] = [];

  //avManifests : AvManifest[] = [];

  private baseUrl: string = 'https://localhost:7101/api';

  constructor(private utubeService: AvManifestService, private http: HttpClient) 
  {}


  //constructor(private http: HttpClient) { }

  // fetchManifests() {
  //   if (!this.id) {
  //     alert("Please enter an ID");
  //     return;
  //   }

  //   this.utubeService.getData("AvManifest/" + this.id).subscribe(response => {
  //     this.videoManifests = response; //.filter((manifest: { type: string; }) => manifest.type === 'video');
  //     this.audioManifests = response; //.filter((manifest: { type: string; }) => manifest.type === 'audio');
  //   });
  // }

  fetchAvManifest() {
    this.http.get<AvManifest>(`${this.baseUrl}/AvManifest/${this.resourceId}`)
      .subscribe(manifest => {
        this.audioStreams = manifest.audioStreams;
        this.videoStreams = manifest.videoStreams;
        manifest.url = `https://www.youtube.com/watch?v=${manifest.id}`;
        
        this.avManifest = manifest;
      });
  }


    // const apiUrl1 = `YOUR_API_ENDPOINT_1/${this.id}`; 
    // const apiUrl2 = `YOUR_API_ENDPOINT_2/${this.id}`;


    // //AvManifest/{avResourceId}
    // this.http.get<VideoManifest[]>(apiUrl1, {}).subscribe(data => this.videoManifests = data || []);
    // this.http.get<AudioManifest[]>(apiUrl2, {}).subscribe(data => this.audioManifests = data || []);


    // submitForm() {
    //   const data = {
    //     name: this.name,
    //     email: this.email,
    //     message: this.message
    //   };
  
    //   const apiUrl = environment.apiUrl; // Get the API URL from environment
  
    //   this.http.post<PostResponse>(apiUrl, data).subscribe({
    //     next: (response) => {
    //       console.log('Success:', response);
    //       this.responseMessage = response.message;
    //       this.errorMessage = null; // Clear any previous errors
    //       this.clearForm(); // Clear the form after successful submission
    //     },
    //     error: (error) => {
    //       console.error('Error:', error);
    //       this.errorMessage = error.message; // Display the error message
    //       this.responseMessage = null; // Clear any previous success messages
    //     }
    //   });

  

  onCheckboxChange(streamId: string, isChecked: boolean, streamsToUpdate: string[]) {
    // if (!manifestTitle)
    //   return;
    if (isChecked) {
      streamsToUpdate.push(streamId);
    } else {
      const index = streamsToUpdate.indexOf(streamId);
      if (index > -1) {
        streamsToUpdate.splice(index, 1);
      }
    }

    //if (streamsToUpdate === this.selectedVStreams) {
      this.selectedVStreams = streamsToUpdate;
    //} else if (streamsToUpdate === this.selectedAStreams) {
      this.selectedAStreams = streamsToUpdate;
    //}
  }

}
