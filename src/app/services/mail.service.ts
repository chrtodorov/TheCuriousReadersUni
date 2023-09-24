import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { Mail } from "../models/mail.model";

@Injectable({
    providedIn: 'root'
})
export class MailService{
    mailURL = environment.MAIL_API;

    constructor(private http: HttpClient){}

    sendEmail(model: any){
        return this.http.post<Mail>(this.mailURL, model)
    }
}