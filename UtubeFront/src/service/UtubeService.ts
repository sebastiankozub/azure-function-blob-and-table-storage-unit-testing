import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AvManifestService {

  private baseUrl: string = 'https://localhost:7101/api';

  constructor(private http: HttpClient) { }

  getData(endpoint: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/${endpoint}`);
  }
}

///GET
////api/AvManifest/{avResourceId}

// @@Injectable({
//   providedIn: 'root'
// })
// export class ManifestService {
//   private apiUrl = 'https://your-api-endpoint.com/api/manifests';

//   constructor(private http: HttpClient) {}

//   getManifests(): Observable<any[]> {
//     return this.http.get<any[]>(this.apiUrl);
//   }
// }