
export abstract class LoadScreen
{
  isLoading = false;

  showLoadScreen(show: boolean) {
    if (show) {
      this.isLoading = true;
      document.getElementById("overlay").style.display = "block";
    }
    else {
      this.isLoading = false;
      document.getElementById("overlay").style.display = "none";
    }
  }
}
