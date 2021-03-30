import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {
  linkedInLink: string;
  gitHubLink: string

  constructor() { } 

  ngOnInit(): void {
    this.linkedInLink = environment.linkedin;
    this.gitHubLink = environment.github;
  }
}