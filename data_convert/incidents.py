import json
from shapely.geometry import mapping, shape

j = json.loads(open("ESA Incidents and Responses 2014 - 2016 Suburb.geojson",'r').read())
f = open('incidents.csv','w')

line = 'id,type,date,long,lat'
f.write(line + '\n')

k = 0
for i in j['features']:
	itype = i['properties']['incident_type']
	date = i['properties']['creation_date']
	coords = shape(i['geometry']).coords[:]
	if date[:4] == '2016' and ('FIRE' in itype or 'EXPLOS' in itype or 'ACCIDENT' in itype or 'BOMB' in itype):
		line = str(k) + ',' + itype + ',' + date + ',' + str(coords[0][0]) + ',' + str(coords[0][1])
		print(line)
		f.write(line + '\n')
		k = k + 1

f.close()
