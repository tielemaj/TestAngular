import { Component, OnInit } from '@angular/core';
import { AccountService } from './services/account.service';
import { User } from '../models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'client';

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser() {
    const userstring = localStorage.getItem('user');
    if (!userstring) return;
    const user: User = JSON.parse(userstring);
    this.accountService.setCurrentUser(user);
  }
}