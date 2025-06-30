from django.shortcuts import render

# Return PlayCanvas build as an HTTP response
def playcanvas(request):
    return render(request, 'index.html')