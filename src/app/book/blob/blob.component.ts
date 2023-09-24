import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, EventEmitter,Input,Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { BlobMetadata } from '../../models/BlobMetadata';

@Component({
  selector: 'app-blob',
  templateUrl: './blob.component.html',
  styleUrls: ['./blob.component.css']
})
export class BlobComponent {
  @Output() blobChanged = new EventEmitter<BlobMetadata>()
  @Input() currentCover: string;

  apiUrl = environment.baseUrl + '/Blobs';
  jwtToken = JSON.parse(sessionStorage.getItem('jwtToken'));

  httpOptions = {
    headers: new HttpHeaders({
      'Authorization': `Bearer ${this.jwtToken}`
    })
  }

  fileToUpload: any;
  imageUrl: any;

  constructor(private httpClient: HttpClient, private toastr: ToastrService) {}

  handleFileInput(event: Event): void {
    this.fileToUpload = (<HTMLInputElement>event.target).files[0];
    
    this.upload(this.fileToUpload)
  }

  upload(file: File) {
    const formData = new FormData();
    formData.append('ImageFile', file);

    this.httpClient.post<BlobMetadata>(this.apiUrl + '/upload', formData, this.httpOptions).subscribe({
      next: (response: BlobMetadata) => {
        this.blobChanged.emit(response);
        this.toastr.success("Upload successfull")
        this.renderImage();
      },
      error: (err: any) => {
        this.toastr.error(err.error);
        this.clearImageProps();
      }
    })
  }
  clearImageProps(){
    this.imageUrl = null;
    this.fileToUpload = null;
  }
  renderImage(){
    let reader = new FileReader();
    reader.onload = (event: any) => {
      this.imageUrl = event.target.result;
    }
    reader.readAsDataURL(this.fileToUpload);
  }
 }